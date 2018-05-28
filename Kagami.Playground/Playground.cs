using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
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
using static Standard.Types.Arrays.ArrayFunctions;
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
      int[] tabStops;
      Hash<string, IObject> watch;
      FolderName packageFolder;

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
            tabStops = array(32, 64, 96, 128);
            textEditor.SelectionTabs = tabStops;
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
            menus.Menu("Insert", "println", (s, evt) => insertText("println ", 0, 0), "^P");
            menus.Menu("Insert", "println interpolated", (s, evt) => insertText("println $\"\"", -1, 0), "^%P");
            menus.Menu("Insert", "print", (s, evt) => insertText("print ", 0, 0));
            menus.Menu("Insert", "put", (s, evt) => insertText("put ", 0, 0));
            menus.Menu("Insert", "peek", (s, evt) => surround("peek(", ")"), "^K");
            menus.Menu("Insert", "Triple quotes", (s, evt) => insertText("\"\"\"\n\"\"\"", -3), "^Q");
            menus.Menu("Insert", "Set", (s, evt) => insertText("«»", -1), "^D9");

            menus.Menu("&Debug");
            menus.Menu("Debug", "Step Into", (s, evt) => stepInto(), "F11");
            menus.Menu("Debug", "Step Over", (s, evt) => stepOver(), "F10");

            menus.CreateMainMenu(this);
            menus.StandardContextEdit(document);

            textEditor.ReassignHandle();
            textEditor.Paint += (s, evt) => paintResults(evt);

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

               var kagamiConfiguration = new CompilerConfiguration { ShowOperations = dumpOperations, Tracing = tracing };
               var complier = new Compiler(textEditor.Text, kagamiConfiguration, context);
               if (complier.Generate().If(out var machine, out var exception))
               {
                  machine.PackageFolder = packageFolder.FullPath;
                  if (execute)
                     if (machine.Execute().IfNot(out exception))
                     {
                        textWriter.WriteLine(exception.Message);
                        exceptionIndex = complier.ExceptionIndex;
                        if (exceptionIndex.IsNone)
                           exceptionIndex = textEditor.SelectionStart.Some();
                     }
                     else
                     {
                        cancelled = context.Cancelled();
                        context.Reset();
                     }

                  stopwatch.Stop();
                  colorizer.Colorize(complier.Tokens);

                  if (exceptionIndex.If(out var index))
                  {
                     var remainingLineLength = getRemainingLineIndex(index);
                     if (remainingLineLength > -1)
                        showException(index, remainingLineLength);
                  }

                  if (dumpOperations && complier.Operations.If(out var operations))
                     textWriter.WriteLine(operations);

                  document.Clean();

                  labelStatus.Text = stopwatch.Elapsed.ToLongString(true) + (cancelled ? " (cancelled)" : "");
               }
               else
               {
                  textWriter.WriteLine(exception.Message);
                  exceptionIndex = complier.ExceptionIndex;
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
         var selectionStart = textEditor.SelectionStart;
         var selectionLength = textEditor.SelectionLength;
         textEditor.Select(index, length);
         textEditor.SelectionColor = Color.White;
         textEditor.SelectionBackColor = Color.Red;
         textEditor.Select(selectionStart, selectionLength);
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

      void paintResults(PaintEventArgs e)
      {
         var font = new Font(textEditor.Font, FontStyle.Bold);

         try
         {
            Colorizer.StopTextBoxUpdate(textEditor);
            var lineFromCharIndex1 = textEditor.GetLineFromCharIndex(textEditor.GetCharIndexFromPosition(new Point(0, 0)));
            var array1 = Enumerable.Range(0, textEditor.Lines.Length).Select(l => "").ToArray();
            var array2 = Enumerable.Range(0, textEditor.Lines.Length).Select(l => 0.0f).ToArray();
            foreach (var result in context.Peeks)
               try
               {
                  var lineFromCharIndex2 = textEditor.GetLineFromCharIndex(result.Key);
                  array1[lineFromCharIndex2] = lineFromCharIndex2 >= lineFromCharIndex1 ? result.Value.VisibleWhitespace(false) : "";
                  array2[lineFromCharIndex2] = textEditor.GetPositionFromCharIndex(result.Key).Y;
               }
               catch { }
            for (var lineNumber = lineFromCharIndex1; lineNumber < textEditor.Lines.Length; ++lineNumber)
            {
               var str = array1[lineNumber];
               var line = textEditor.Lines[lineNumber];
               var sizeF1 = e.Graphics.MeasureString(line, font);
               var y = array2[lineNumber];
               if (str.IsNotEmpty())
               {
                  var sizeF2 = e.Graphics.MeasureString(str, font);
                  var val1 = textEditor.ClientRectangle.Width - sizeF1.Width;
                  sizeF2.Width = Math.Min(val1, sizeF2.Width) - 0.0f;
                  var point = new PointF(e.Graphics.ClipBounds.Width - sizeF2.Width, y);
                  var rect = new RectangleF(point.X, point.Y + 1f, sizeF2.Width, sizeF2.Height - 2f);
                  e.Graphics.FillRectangle(Brushes.LightGreen, rect);
                  using (var pen = new Pen(SystemColors.ButtonShadow, 1f))
                     e.Graphics.DrawRectangle(pen, getRectangle(rect));
                  var format = new StringFormat(StringFormatFlags.LineLimit);
                  e.Graphics.DrawString(str, font, Brushes.Black, point, format);
               }

               var size = getSize(e.Graphics.MeasureString("\t", font));
               if (line.Matches("^ /(/t1%4)").If(out var matcher))
               {
                  var x1 = 8;
                  var num1 = textEditor.GetPositionFromCharIndex(textEditor.GetFirstCharIndexFromLine(lineNumber)).Y + size.Height / 2;
                  var index1 = matcher.FirstGroup.Length - 1;
                  var tabStop1 = tabStops[index1];
                  using (var pen = new Pen(Color.LightGray, 1f))
                  {
                     pen.CustomEndCap = new AdjustableArrowCap(3f, 3f, true);
                     e.Graphics.DrawLine(pen, x1, num1, x1 + tabStop1, num1);
                  }

                  using (var pen = new Pen(Color.LightGray, 1f))
                     for (var index2 = 0; index2 < index1; ++index2)
                     {
                        var tabStop2 = tabStops[index2];
                        var num2 = x1 + tabStop2;
                        e.Graphics.DrawLine(pen, num2, num1 - 8, num2, num1 + 8);
                     }
               }
            }
         }
         catch { }
         finally
         {
            font?.Dispose();
            Colorizer.ResumeTextBoxUpdate(textEditor);
         }
      }

      static Rectangle getRectangle(RectangleF rect)
      {
         return new Rectangle((int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height);
      }

      static Size getSize(SizeF size) => new Size((int)size.Width, (int)size.Height);

      void textEditor_TextChanged(object sender, EventArgs e)
      {
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
         if (document.FileName.If(out var fileName) && !((FileName)fileName).Exists())
            try
            {
               configurationFile = PLAYGROUND_CONFIGURATION_FILE1;
               playgroundConfiguration.LastFile = fileName;
               playgroundConfiguration.DefaultFolder = FolderName.Current;
               playgroundConfiguration.FontName = textEditor.Font.Name;
               playgroundConfiguration.FontSize = textEditor.Font.Size;
               configurationFile.Text = ObjectGraph.Serialize(playgroundConfiguration).ToString();
            }
            catch (Exception exception)
            {
               MessageBox.Show(exception.Message);
            }
      }

      void Playground_KeyUp(object sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Escape)
         {
            context.Cancel();
            e.Handled = true;
         }
      }
   }
}