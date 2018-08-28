using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Kagami.Library;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Computer;
using Standard.ObjectGraphs;
using Standard.Types.Arrays;
using Standard.Types.Collections;
using Standard.Types.Dates;
using Standard.Types.Maybe;
using Standard.Types.Numbers;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using Standard.WinForms.Consoles;
using Standard.WinForms.Documents;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Playground
{
   public partial class Playground : Form
   {
      const string PLAYGROUND_FONT_NAME = "Consolas";
      const float PLAYGROUND_FONT_SIZE = 14f;
      const string PLAYGROUND_CONFIGURATION_FILE1 = @"E:\Configurations\Kagami\Kagami.configuration";
      const string PLAYGROUND_CONFIGURATION_FILE2 = @"E:\Configurations\Kagami\Kagami.configuration";
      const string PLAYGROUND_PACKAGE_FOLDER = @"E:\Enterprise\Configurations\Kagami\Packages";

      Document document;
      TextBoxConsole outputConsole;
      TextWriter textWriter;
      TextReader textReader;
      bool locked;
      bool manual;
      Stopwatch stopwatch;
      FileName configurationFile;
      PlaygroundConfiguration playgroundConfiguration;
      PlaygroundContext context;
      Colorizer colorizer;
      //bool debugging;
      bool dumpOperations;
      bool tracing;
      IMaybe<int> exceptionIndex;
      bool cancelled;
      Hash<string, IObject> watch;
      FolderName packageFolder;
      IMaybe<ExceptionData> exceptionData;

      public Playground()
      {
         InitializeComponent();
      }

      void Playground_Load(object sender, EventArgs e)
      {
         try
         {
            configurationFile = PLAYGROUND_CONFIGURATION_FILE1;
            if (configurationFile.Exists())
               if (ObjectGraph.FromFile(configurationFile).Object<PlaygroundConfiguration>()
                  .If(out playgroundConfiguration, out var exception)) { }
               else
                  throw exception;
            else
            {
               configurationFile = PLAYGROUND_CONFIGURATION_FILE2;
               if (configurationFile.Exists())
                  if (ObjectGraph.FromFile(configurationFile).Object<PlaygroundConfiguration>()
                     .If(out playgroundConfiguration, out var exception)) { }
                  else
                     throw exception;
               else
                  playgroundConfiguration = new PlaygroundConfiguration
                  {
                     DefaultFolder = FolderName.Current, FontName = PLAYGROUND_FONT_NAME, FontSize = PLAYGROUND_FONT_SIZE,
                     PackageFolder = PLAYGROUND_PACKAGE_FOLDER
                  };
            }
         }
         catch (Exception exception)
         {
            labelStatus.Text = exception.Message;
         }

         packageFolder = playgroundConfiguration.PackageFolder;

         outputConsole = new TextBoxConsole(this, textConsole, playgroundConfiguration.FontName, playgroundConfiguration.FontSize,
            TextBoxConsole.ConsoleColorType.Quick);
         textWriter = outputConsole.Writer();
         textReader = outputConsole.Reader();
         context = new PlaygroundContext(textWriter, textReader);
         colorizer = new Colorizer(textEditor);

         try
         {
            exceptionData = none<ExceptionData>();
            textEditor.SetTabs(32, 64, 96, 128);
            document = new Document(this, textEditor, ".kagami", "Kagami", playgroundConfiguration.FontName,
               playgroundConfiguration.FontSize);
            document.StandardMenus();

            var menus = document.Menus;

            menus.Menu("Edit", "Duplicate", (s, evt) => duplicate(), "^D");
            menus.Menu("Edit", "Indent", (s, evt) => indent(), "^I");
            menus.Menu("Edit", "Unindent", (s, evt) => unindent(), "^%I");
            menus.Menu("&Build");
            menus.Menu("Build", "Run", (s, evt) => run(), "F5");
            menus.Menu("Build", "Manual", (s, evt) =>
            {
               manual = !manual;
               ((ToolStripMenuItem)s).Checked = manual;
            }, "^F5");
            menus.Menu("Build", "Dump operations", (s, evt) =>
            {
               dumpOperations = !dumpOperations;
               ((ToolStripMenuItem)s).Checked = dumpOperations;
            });
            menus.Menu("Build", "Trace", (s, evt) =>
            {
               tracing = !tracing;
               ((ToolStripMenuItem)s).Checked = tracing;
            }, "^T");
            menus.Menu("&Insert");
            menus.Menu("Insert", "open sys", (s, evt) => insertText("open sys\n\n", 0, 0), "^%S");
            menus.Menu("Insert", "open math", (s, evt) => insertText("open math\n\n", 0, 0), "^%M");
            menus.Menu("Insert", "println()", (s, evt) => insertText("println()", -1, 0), "^P");
            menus.Menu("Insert", "println() interpolated", (s, evt) => insertText("println($\"\")", -2, 0), "^%P");
            menus.Menu("Insert", "print()", (s, evt) => insertText("print()", -1, 0));
            menus.Menu("Insert", "put()", (s, evt) => insertText("put()", -1, 0));
            menus.Menu("Insert", "peek()", (s, evt) => surround("peek(", ")"), "^K");
            menus.Menu("Insert", "Triple quotes", (s, evt) => insertText("\"\"\"\n\"\"\"", -3), "^Q");

            menus.Menu("&Debug");
            menus.Menu("Debug", "Step Into", (s, evt) => stepInto(), "F11");
            menus.Menu("Debug", "Step Over", (s, evt) => stepOver(), "F10");

            menus.CreateMainMenu(this);
            menus.StandardContextEdit(document);

            textEditor.SetLeftMargin(70);
            textEditor.ReassignHandle();
            textEditor.Paint += (s, evt) => paint(evt);
            textEditor.AnnotationFont = new Font(textEditor.Font, FontStyle.Bold);

            locked = false;
            manual = false;
            //debugging = false;
            dumpOperations = false;
            tracing = false;
            stopwatch = new Stopwatch();
            exceptionIndex = none<int>();
            cancelled = false;
            watch = new Hash<string, IObject>();
            if (playgroundConfiguration.LastFile != null)
               document.Open(playgroundConfiguration.LastFile);
         }
         catch (Exception exception)
         {
            textWriter.WriteLine(exception.Message);
         }
      }

      void run() => update(true, true);

      void update(bool execute, bool fromMenu)
      {
         if (!locked && textEditor.TextLength != 0)
         {
            locked = true;
            if (manual)
            {
               labelStatus.Text = "running...";
               Application.DoEvents();
            }
            else if (fromMenu)
               document.Save();

            try
            {
               textConsole.Clear();
               context.ClearPeeks();
               stopwatch.Reset();
               stopwatch.Start();
               exceptionIndex = none<int>();
               exceptionData = none<ExceptionData>();

               var kagamiConfiguration = new CompilerConfiguration { ShowOperations = dumpOperations, Tracing = tracing };
               var compiler = new Compiler(textEditor.Text, kagamiConfiguration, context);
               if (compiler.Generate().If(out var machine, out var exception))
               {
                  machine.PackageFolder = packageFolder.FullPath;
                  if (execute)
                     if (machine.Execute().IfNot(out exception))
                     {
                        textWriter.WriteLine(exception.Message);
                        exceptionIndex = compiler.ExceptionIndex;
                        if (exceptionIndex.IsNone)
                           exceptionIndex = textEditor.SelectionStart.Some();
                     }
                     else
                     {
                        cancelled = context.Cancelled();
                        context.Reset();
                     }

                  stopwatch.Stop();
                  var state = textEditor.StopAutoscrollingAlways();
                  try
                  {
                     colorizer.Colorize(compiler.Tokens);
                  }
                  finally
                  {
                     textEditor.ResumeAutoscrollingAlways(state);
                  }

                  if (exceptionIndex.If(out var index))
                  {
                     var remainingLineLength = getRemainingLineIndex(index);
                     if (remainingLineLength > -1)
                        showException(index, remainingLineLength);
                  }

                  if (dumpOperations && compiler.Operations.If(out var operations))
                     textWriter.WriteLine(operations);

                  document.Clean();

                  labelStatus.Text = stopwatch.Elapsed.ToLongString(true) + (cancelled ? " (cancelled)" : "");
               }
               else
               {
                  textWriter.WriteLine(exception.Message);
                  exceptionIndex = compiler.ExceptionIndex;
                  if (exceptionIndex.IsNone)
                     exceptionIndex = textEditor.SelectionStart.Some();
                  if (exceptionIndex.If(out var index))
                  {
                     var remainingLineLength = getRemainingLineIndex(index);
                     if (remainingLineLength > -1)
                        showException(index, remainingLineLength);
                  }
               }
            }
            catch (Exception exception)
            {
               textWriter.WriteLine(exception.Message);
            }
            finally
            {
               locked = false;
            }
         }
      }

      void stepInto() => Machine.Current?.Step(true, watch);

      void stepOver() => Machine.Current?.Step(false, watch);

      int getRemainingLineIndex(int index)
      {
         for (var i = index; i < textEditor.TextLength; i++)
            switch (textEditor.Text[i])
            {
               case '\n':
               case '\r':
                  if (index != i)
                     return i - index;

                  break;
            }

         return textEditor.TextLength - index;
      }

      void showException(int index, int length)
      {
         exceptionData = new ExceptionData(index, length).Some();
         textEditor.Invalidate();
      }

      void duplicate()
      {
         var text = "";
         if (textEditor.SelectionLength == 0)
         {
            var number = textEditor.CurrentLineIndex();
            if (textEditor.AtEnd() && textEditor.Text.IsMatch("/n $"))
               number--;
            var lines = textEditor.Lines;
            if (number.Between(0).Until(lines.Length))
               text = lines[number].Copy();
         }
         else
            text = textEditor.SelectedText;

         textEditor.AppendAtEnd(text, "/n");
      }

      void indent()
      {
         try
         {
            var lines = textEditor.Lines;
            var obj = (object)linesFromSelection();
            var num1 = obj is Some<ValueTuple<string[], int>> ? 1 : 0;
            var some = num1 != 0 ? (Some<ValueTuple<string[], int>>)obj : new Some<ValueTuple<string[], int>>();
            if (num1 == 0)
               return;

            var valueTuple = some.Value;
            var strArray = valueTuple.Item1;
            var num2 = valueTuple.Item2;
            ValueTuple<int, int> saved = saveSelection();
            for (var index = 0; index < strArray.Length; ++index)
               textEditor.Lines[index + num2] = $"\t{strArray[index]}";
            textEditor.Lines = lines;
            restoreSelection(saved);
         }
         catch (Exception ex)
         {
            textWriter.WriteLine(ex.Message);
         }
      }

      void unindent()
      {
         try
         {
            var lines = textEditor.Lines;
            var obj = (object)linesFromSelection();
            var num1 = obj is Some<ValueTuple<string[], int>> ? 1 : 0;
            var some = num1 != 0 ? (Some<ValueTuple<string[], int>>)obj : new Some<ValueTuple<string[], int>>();
            if (num1 == 0)
               return;

            var valueTuple = some.Value;
            var strArray = valueTuple.Item1;
            var num2 = valueTuple.Item2;
            ValueTuple<int, int> saved = saveSelection();
            for (var index = 0; index < strArray.Length; ++index)
               if (strArray[index].Matches("^ /t /@").If(out var matcher))
                  lines[index + num2] = matcher.FirstGroup;

            textEditor.Lines = lines;
            restoreSelection(saved);
         }
         catch (Exception ex)
         {
            textWriter.WriteLine(ex.Message);
         }
      }

      IMaybe<(string[] lines, int index)> linesFromSelection()
      {
         var lineFromCharIndex1 = textEditor.GetLineFromCharIndex(textEditor.SelectionStart);
         var lineFromCharIndex2 = textEditor.GetLineFromCharIndex(textEditor.SelectionStart + textEditor.SelectionLength);
         var lines = textEditor.Lines;
         if (lineFromCharIndex1.Between(0).Until(lines.Length) && lineFromCharIndex2.Between(0).Until(lines.Length))
            return (lines.RangeOf(lineFromCharIndex1, lineFromCharIndex2), lineFromCharIndex1).Some();

         return none<(string[], int)>();
      }

      (int start, int length) saveSelection() => (textEditor.SelectionStart, textEditor.SelectionLength);

      void restoreSelection((int start, int length) saved) => textEditor.Select(saved.start, saved.length);

      void insertText(string text, int selectionOffset, int length = -1)
      {
         textEditor.SelectedText = text;
         textEditor.SelectionStart += selectionOffset;
         if (length <= -1)
            return;

         textEditor.SelectionLength = length;
      }

      void surround(string before, string after)
      {
         var selectedText = textEditor.SelectedText;
         textEditor.SelectedText = $"{before}{selectedText}{after}";
      }

      void insertDelimiterText(string delimiter, int selectionOffset, int length, int halfLength = -1)
      {
         if (textEditor.SelectionLength == 0)
            insertText(delimiter, selectionOffset, length);
         else
         {
            var selectedText = textEditor.SelectedText;
            if (halfLength == -1)
               halfLength = delimiter.Length / 2;
            textEditor.SelectedText = delimiter.Take(halfLength) + selectedText + delimiter.Skip(halfLength);
         }
      }

      string textAtInsert(int take, int skip = 0) => textEditor.Text.Skip(textEditor.SelectionStart + skip).Take(take);

      void setTextAtInsert(int take, int skip = 0, string text = "")
      {
         textEditor.Select(textEditor.SelectionStart + skip, take);
         textEditor.SelectedText = text;
         textEditor.Select(textEditor.SelectionStart + skip, 0);
      }

      void moveSelectionRelative(int amount = 1) => textEditor.SelectionStart += amount;

      void paint(PaintEventArgs e)
      {
         try
         {
            var peeks = new Hash<int, string>();
            foreach (var result in context.Peeks)
               try
               {
                  var lineIndex = textEditor.GetLineFromCharIndex(result.Key);
                  peeks[lineIndex] = result.Value;
               }
               catch { }

            foreach (var (lineNumber, line, _) in textEditor.VisibleLines)
            {
               if (peeks.ContainsKey(lineNumber))
               {
                  var str = peeks[lineNumber];
                  str = sizedAnnotation(e.Graphics, line, str, textEditor.ClientSize.Width, textEditor.Font,
                     textEditor.AnnotationFont);
                  textEditor.AnnotateAt(e.Graphics, lineNumber, str, Color.Black, Color.LightGreen, Color.Black);
               }

               textEditor.DrawTabLines(e.Graphics);
               textEditor.DrawLineNumbers(e.Graphics, Color.Black, Color.LightGreen);
               if (textEditor.SelectionLength == 0)
                  textEditor.DrawCurrentLineBar(e.Graphics, Color.Black, Color.White, alpha: 0);

               if (exceptionData.If(out var data))
               {
                  var rectangle = textEditor.RectangleFrom(e.Graphics, data.Index, data.Length, false);
                  textEditor.DrawWavyUnderline(e.Graphics, rectangle, Color.Red);
               }
            }
         }
         catch { }
      }

      static int getWidth(Graphics graphics, string text, Font font) => (int)graphics.MeasureString(text, font).Width;

      static string sizedAnnotation(Graphics graphics, string line, string annotation, int width, Font font, Font annotationFont)
      {
         var diff = 80;
         var lineWidth = getWidth(graphics, line, font);
         var remainingWidth = width - lineWidth - diff;
         var dots = "…";
         if (remainingWidth <= diff)
            return dots;
         else
         {
            var annotationWidth = getWidth(graphics, annotation, annotationFont);
            if (annotationWidth <= remainingWidth)
               return annotation;
            else
            {
               while (annotationWidth > remainingWidth)
               {
                  annotation = annotation.Skip(-2) + dots;
                  annotationWidth = getWidth(graphics, annotation, annotationFont);
               }

               return annotation;
            }
         }
      }

      void textEditor_TextChanged(object sender, EventArgs e)
      {
         exceptionData = none<ExceptionData>();

         if (document != null)
         {
            update(!manual, false);
            document.Dirty();
         }
      }

      void textEditor_KeyPress(object sender, KeyPressEventArgs e)
      {
         switch (e.KeyChar)
         {
            case '"':
               if (textAtInsert(1) == "\"")
               {
                  moveSelectionRelative();
                  e.Handled = true;
                  break;
               }

               insertDelimiterText("\"\"", -1, 0);
               e.Handled = true;
               break;
            case '(':
               insertDelimiterText("()", -1, 0);
               e.Handled = true;
               break;
            case ')':
               if (textAtInsert(1) != ")")
                  break;

               moveSelectionRelative();
               e.Handled = true;
               break;
            case ',':
               if (textAtInsert(1) != ",")
                  break;

               moveSelectionRelative();
               e.Handled = true;
               break;
            case ';':
               if (textAtInsert(1) != ";")
                  break;

               moveSelectionRelative();
               e.Handled = true;
               break;
            case '[':
               insertDelimiterText("[]", -1, 0);
               e.Handled = true;
               break;
            case ']':
               if (textAtInsert(1) != "]")
                  break;

               moveSelectionRelative();
               e.Handled = true;
               break;
            case '{':
               insertDelimiterText("{}", -1, 0);
               e.Handled = true;
               break;
            case '}':
               if (textAtInsert(1) != "}")
                  break;

               moveSelectionRelative();
               e.Handled = true;
               break;
         }
      }

      void textEditor_KeyUp(object sender, KeyEventArgs e)
      {
         switch (e.KeyCode)
         {
            case Keys.Escape:
               if (textAtInsert(1) == "'" && textAtInsert(1, -1) == "'")
               {
                  e.Handled = true;
                  setTextAtInsert(1);
                  break;
               }

               if (textAtInsert(1) == "\"" && textAtInsert(1, -1) == "\"")
               {
                  e.Handled = true;
                  setTextAtInsert(1);
                  break;
               }

               if (textAtInsert(1) == ")" && textAtInsert(1, -1) == "(")
               {
                  e.Handled = true;
                  setTextAtInsert(1);
                  break;
               }

               if (textAtInsert(1) == "]" && textAtInsert(1, -1) == "[")
               {
                  e.Handled = true;
                  setTextAtInsert(1);
                  break;
               }

               if (textAtInsert(1) != "}" || textAtInsert(1, -1) != "{")
                  break;

               e.Handled = true;
               setTextAtInsert(1);
               break;
            case Keys.F1:
               if (getWord().If(out var begin) && findWord(begin).If(out var source))
                  insertText(source.Skip(begin.Length), 0);
               e.Handled = true;
               break;
         }
      }

      IMaybe<string> getWord()
      {
         return textEditor.Text.Take(textEditor.SelectionStart).Matches("/(/w+) $").Map(m => m.FirstGroup);
      }

      IMaybe<string> findWord(string begin)
      {
         return textEditor.Text.Split("-/w+").Distinct().Where(word => word.StartsWith(begin)).FirstOrNone();
      }

      void Playground_FormClosing(object sender, FormClosingEventArgs e)
      {
/*         if (document.FileName.If(out var fileName) && ((FileName)fileName).Exists())
            try
            {
               //configurationFile = PLAYGROUND_CONFIGURATION_FILE1;
               playgroundConfiguration.LastFile = fileName;
               playgroundConfiguration.DefaultFolder = FolderName.Current;
               playgroundConfiguration.FontName = textEditor.Font.Name;
               playgroundConfiguration.FontSize = textEditor.Font.Size;
               configurationFile.Text = ObjectGraph.Serialize(playgroundConfiguration).ToString();
            }
            catch (Exception exception)
            {
               MessageBox.Show(exception.Message);
            }*/
      }

      void Playground_KeyUp(object sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Escape)
         {
            context.Cancel();
            e.Handled = true;
         }
      }

      void textEditor_SelectionChanged(object sender, EventArgs e) => textEditor.Invalidate();
   }
}