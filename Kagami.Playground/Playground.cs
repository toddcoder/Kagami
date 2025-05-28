using System.Diagnostics;
using Core.Arrays;
using Kagami.Library;
using Kagami.Library.Runtime;
using Core.Computers;
using Core.Collections;
using Core.Dates;
using Core.Enumerables;
using Core.Exceptions;
using Core.Monads;
using Core.Numbers;
using Core.Matching;
using Core.Strings;
using Core.WinForms.Consoles;
using Core.WinForms.Documents;
using static Core.Monads.MonadFunctions;

namespace Kagami.Playground;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public partial class Playground : Form
{
   protected const string KAGAMI_EXCEPTION_PROMPT = "Kagami exception >>> ";

   protected Document document = null!;
   protected TextBoxConsole outputConsole = null!;
   protected TextWriter textWriter = null!;
   protected TextReader textReader = null!;
   protected bool locked;
   protected bool manual;
   protected Stopwatch stopwatch = null!;
   protected PlaygroundConfiguration playgroundConfiguration = null!;
   protected PlaygroundContext context = null!;
   protected Colorizer colorizer = null!;
   protected bool dumpOperations;
   protected bool tracing;
   protected Maybe<int> _exceptionIndex = nil;
   protected bool cancelled;
   protected FolderName packageFolder = null!;
   protected Maybe<ExceptionData> _exceptionData = nil;
   protected int firstEditorLine;

   public Playground()
   {
      InitializeComponent();
      firstEditorLine = 0;
   }

   protected void Playground_Load(object sender, EventArgs e)
   {
      var _configuration = PlaygroundConfiguration.Retrieve();
      if (_configuration is (true, var configuration))
      {
         playgroundConfiguration = configuration;
      }
      else
      {
         playgroundConfiguration = new PlaygroundConfiguration();
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
         _exceptionData = nil;
         document = new Document(this, textEditor, ".kagami", "Kagami", playgroundConfiguration.FontName, playgroundConfiguration.FontSize);
         /*{
            AboutToDisplayFile =
            {
               Handler = i =>
               {
                  if (i.File is (true, var file))
                  {
                     Console.WriteLine(file.FullPath);
                  }
                  else
                  {
                     Console.WriteLine("No file to display");
                  }
               }
            }
         };*/
         var menus = document.Menus;
         menus.Menu("&File");
         menus.Menu("File", "&New", (_, _) =>
         {
            textEditor.ClearModificationGlyphs();
            document.New();
         });
         menus.Menu("File", "&Open", (_, _) =>
         {
            textEditor.ClearModificationGlyphs();
            document.Open();
         }, "^O");
         menus.Menu("File", "&Save", (_, _) =>
         {
            textEditor.SetToSavedGlyphs();
            document.Save();
         }, "^S");
         menus.Menu("File", "Save As", (_, _) =>
         {
            textEditor.SetToSavedGlyphs();
            document.SaveAs();
         });
         menus.Menu("File", "Exit", (_, _) => Close(), "%F4");

         document.StandardEditMenu();

         menus.Menu("Edit", "Duplicate", (_, _) => duplicate(), "^D");
         menus.Menu("Edit", "Create Block", (_, _) => createBlock(), "^B");
         menus.Menu("&Build");
         menus.Menu("Build", "Run", (_, _) => run(), "F5");
         menus.Menu("Build", "Manual", (s, _) =>
         {
            manual = !manual;
            ((ToolStripMenuItem)s!).Checked = manual;
         }, "^F5");
         menus.Menu("Build", "Dump operations", (s, _) =>
         {
            dumpOperations = !dumpOperations;
            ((ToolStripMenuItem)s!).Checked = dumpOperations;
         });
         menus.Menu("Build", "Trace", (s, _) =>
         {
            tracing = !tracing;
            ((ToolStripMenuItem)s!).Checked = tracing;
         }, "^T");
         menus.Menu("&Insert");
         menus.Menu("Insert", "open sys", (_, _) => insertText("open sys\n\n", 0, 0), "^%S");
         menus.Menu("Insert", "open math", (_, _) => insertText("open math\n\n", 0, 0), "^%M");
         menus.Menu("Insert", "println()", (_, _) => insertText("println()", -1, 0), "^P");
         menus.Menu("Insert", "println() interpolated", (_, _) => insertText("println($\"\")", -2, 0), "^%P");
         menus.Menu("Insert", "print()", (_, _) => insertText("print()", -1, 0));
         menus.Menu("Insert", "put()", (_, _) => insertText("put()", -1, 0));
         menus.Menu("Insert", "peek()", (_, _) => surround("peek(", ")"), "^K");
         menus.Menu("Insert", "Triple quotes", (_, _) => insertText("\"\"\"\n\"\"\"", -3), "^Q");
         menus.Menu("Insert", "List", (_, _) => insertText("⌈⌉", -1), "^L");
         menus.Menu("Insert", "Set", (_, _) => insertText("⎩⎭", -1), "^E");

         menus.Menu("&Debug");
         menus.Menu("Debug", "Step Into", (_, _) => stepInto(), "F11");
         menus.Menu("Debug", "Step Over", (_, _) => stepOver(), "F10");

         menus.CreateMainMenu(this);
         menus.StandardContextEdit(document);

         textEditor.SetTabs(32, 64, 96, 128);
         textEditor.SetLeftMargin(70);
         textEditor.ReassignHandle();
         textEditor.Paint += (_, evt) => paint(evt);
         textEditor.AnnotationFont = new Font(textEditor.Font, FontStyle.Bold);

         locked = false;
         manual = false;
         dumpOperations = false;
         tracing = false;
         stopwatch = new Stopwatch();
         _exceptionIndex = nil;
         cancelled = false;
         document.Open(playgroundConfiguration.LastFile);
      }
      catch (Exception exception)
      {
         textWriter.WriteLine(exception.Message);
      }
   }

   protected void run() => update(true, true);

   protected void update(bool execute, bool fromMenu)
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
         {
            document.Save();
         }

         try
         {
            textConsole.Clear();
            context.ClearPeeks();
            stopwatch.Reset();
            stopwatch.Start();
            _exceptionIndex = nil;
            _exceptionData = nil;

            var kagamiConfiguration = new CompilerConfiguration { ShowOperations = dumpOperations, Tracing = tracing };
            var compiler = new Compiler(textEditor.Text, kagamiConfiguration, context);
            var _machine = compiler.Generate();
            if (_machine is (true, var machine))
            {
               machine.PackageFolder = packageFolder.FullPath;
               if (execute)
               {
                  var _result = machine.Execute();
                  if (!_result)
                  {
                     textWriter.WriteLine(KAGAMI_EXCEPTION_PROMPT + _result.Exception.Message);
                     _exceptionIndex = compiler.ExceptionIndex;
                     if (!_exceptionIndex)
                     {
                        _exceptionIndex = textEditor.SelectionStart;
                     }
                  }
                  else
                  {
                     cancelled = context.Cancelled();
                     context.Reset();
                  }
               }

               stopwatch.Stop();
               var state = textEditor.StopAutoScrollingAlways();
               try
               {
                  colorizer.Colorize(compiler.Tokens);
               }
               finally
               {
                  textEditor.ResumeAutoScrollingAlways(state);
               }

               if (_exceptionIndex is (true, var index))
               {
                  var remainingLineLength = getRemainingLineIndex(index);
                  if (remainingLineLength > -1)
                  {
                     showException(index, remainingLineLength);
                  }
               }

               if (dumpOperations && compiler.Operations is (true, var operations))
               {
                  textWriter.WriteLine(operations);
               }

               document.Clean();

               labelStatus.Text = stopwatch.Elapsed.ToLongString(true) + (cancelled ? " (cancelled)" : "");
            }
            else
            {
               textWriter.WriteLine(KAGAMI_EXCEPTION_PROMPT + _machine.Exception.Message);
               _exceptionIndex = compiler.ExceptionIndex;
               if (!_exceptionIndex)
               {
                  _exceptionIndex = textEditor.SelectionStart;
               }

               if (_exceptionIndex is (true, var index))
               {
                  var remainingLineLength = getRemainingLineIndex(index);
                  if (remainingLineLength > -1)
                  {
                     showException(index, remainingLineLength);
                  }
               }
            }
         }
         catch (Exception exception)
         {
            textWriter.WriteLine(KAGAMI_EXCEPTION_PROMPT + exception.Message);
         }
         finally
         {
            locked = false;
         }
      }
   }

   protected static void stepInto() => Machine.Current.Value.Step();

   protected static void stepOver() => Machine.Current.Value.Step();

   protected int getRemainingLineIndex(int index)
   {
      for (var i = index; i < textEditor.TextLength; i++)
      {
         switch (textEditor.Text[i])
         {
            case '\n':
            case '\r':
               if (index != i)
               {
                  return i - index;
               }

               break;
         }
      }

      return textEditor.TextLength - index;
   }

   protected void showException(int index, int length)
   {
      _exceptionData = new ExceptionData(index, length).Some();
      textEditor.Invalidate();
   }

   protected void duplicate()
   {
      var text = "";
      if (textEditor.SelectionLength == 0)
      {
         var number = textEditor.CurrentLineIndex();
         if (textEditor.AtEnd() && textEditor.Text.IsMatch("/n $"))
         {
            number--;
         }

         var lines = textEditor.Lines;
         if (number.Between(0).Until(lines.Length))
         {
            text = new string([.. lines[number]]);
         }
      }
      else
      {
         text = textEditor.SelectedText;
      }

      textEditor.AppendAtEnd(text, "/n");
   }

   protected void createBlock()
   {
      var (selectedLines, _) = textEditor.SelectedLines();
      if (selectedLines.Length > 0)
      {
         var selectedLine = selectedLines[0];
         var whitespace = selectedLine.KeepWhile(i => char.IsWhiteSpace(i[0]));
         textEditor.SelectedText = $"{{\n{whitespace}\t\n{whitespace}}}";
         textEditor.SelectionStart -= 2 + whitespace.Length;
      }
      else
      {
         textEditor.SelectedText = "{\n\t\n}";
         textEditor.SelectionStart -= 2;
      }
   }

   protected Maybe<(string[] lines, int index)> linesFromSelection()
   {
      var lineFromCharIndex1 = textEditor.GetLineFromCharIndex(textEditor.SelectionStart);
      var lineFromCharIndex2 = textEditor.GetLineFromCharIndex(textEditor.SelectionStart + textEditor.SelectionLength);
      var lines = textEditor.Lines;
      if (lineFromCharIndex1.Between(0).Until(lines.Length) && lineFromCharIndex2.Between(0).Until(lines.Length))
      {
         return (lines.RangeOf(lineFromCharIndex1, lineFromCharIndex2), lineFromCharIndex1);
      }

      return nil;
   }

   protected (int start, int length) saveSelection() => (textEditor.SelectionStart, textEditor.SelectionLength);

   protected void restoreSelection((int start, int length) saved) => textEditor.Select(saved.start, saved.length);

   protected void insertText(string text, int selectionOffset, int length = -1)
   {
      textEditor.SelectedText = text;
      textEditor.SelectionStart += selectionOffset;
      if (length > -1)
      {
         textEditor.SelectionLength = length;
      }
   }

   protected void surround(string before, string after)
   {
      var selectedText = textEditor.SelectedText;
      textEditor.SelectedText = $"{before}{selectedText}{after}";
   }

   protected void insertDelimiterText(string delimiter, int selectionOffset, int length, int halfLength = -1)
   {
      if (textEditor.SelectionLength == 0)
      {
         insertText(delimiter, selectionOffset, length);
      }
      else
      {
         var selectedText = textEditor.SelectedText;
         if (halfLength == -1)
         {
            halfLength = delimiter.Length / 2;
         }

         textEditor.SelectedText = delimiter.Keep(halfLength) + selectedText + delimiter.Drop(halfLength);
      }
   }

   protected string textAtInsert(int take, int skip = 0) => textEditor.Text.Drop(textEditor.SelectionStart + skip).Keep(take);

   protected void setTextAtInsert(int take, int skip = 0, string text = "")
   {
      textEditor.Select(textEditor.SelectionStart + skip, take);
      textEditor.SelectedText = text;
      textEditor.Select(textEditor.SelectionStart + skip, 0);
   }

   protected void moveSelectionRelative(int amount = 1) => textEditor.SelectionStart += amount;

   protected void paint(PaintEventArgs e)
   {
      try
      {
         var peeks = new Hash<int, string>();
         foreach (var (key, value) in context.Peeks)
         {
            try
            {
               var lineIndex = textEditor.GetLineFromCharIndex(key);
               peeks[lineIndex] = value;
            }
            catch
            {
            }
         }

         if (textEditor.TextLength == 0)
         {
            return;
         }

         foreach (var (lineNumber, line, _) in textEditor.VisibleLines)
         {
            if (peeks.ContainsKey(lineNumber))
            {
               var str = peeks[lineNumber];
               str = sizedAnnotation(e.Graphics, line, str, textEditor.ClientSize.Width, textEditor.Font, textEditor.AnnotationFont);
               textEditor.AnnotateAt(e.Graphics, lineNumber, str, Color.Black, Color.LightGreen, Color.Black);
            }

            textEditor.DrawTabLines(e.Graphics);
            textEditor.DrawLineNumbers(e.Graphics, Color.Black, Color.White);
            if (textEditor.SelectionLength == 0)
            {
               textEditor.DrawCurrentLineBar(e.Graphics, Color.Black, Color.White, alpha: 0);
            }

            textEditor.DrawModificationGlyphs(e.Graphics);

            if (_exceptionData is (true, var exceptionData))
            {
               var rectangle = textEditor.RectangleFrom(e.Graphics, exceptionData.Index, exceptionData.Length, false);
               textEditor.DrawWavyUnderline(e.Graphics, rectangle, Color.Red);
            }
         }
      }
      catch
      {
      }
   }

   protected static int getWidth(Graphics graphics, string text, Font font) => (int)graphics.MeasureString(text, font).Width;

   protected static string sizedAnnotation(Graphics graphics, string line, string annotation, int width, Font font, Font annotationFont)
   {
      var diff = 80;
      var lineWidth = getWidth(graphics, line, font);
      var remainingWidth = width - lineWidth - diff;
      var dots = "…";
      if (remainingWidth <= diff)
      {
         return dots;
      }
      else
      {
         var annotationWidth = getWidth(graphics, annotation, annotationFont);
         if (annotationWidth > remainingWidth)
         {
            while (annotationWidth > remainingWidth)
            {
               annotation = annotation.Drop(-2) + dots;
               annotationWidth = getWidth(graphics, annotation, annotationFont);
            }
         }

         return annotation;
      }
   }

   protected void textEditor_TextChanged(object sender, EventArgs e)
   {
      try
      {
         _exceptionData = nil;
         update(!manual, false);
         document.Dirty();
      }
      catch
      {
      }
   }

   protected void textEditor_KeyPress(object sender, KeyPressEventArgs e)
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
            {
               break;
            }

            moveSelectionRelative();
            e.Handled = true;
            break;
         case ',':
            if (textAtInsert(1) != ",")
            {
               break;
            }

            moveSelectionRelative();
            e.Handled = true;
            break;
         case ';':
            if (textAtInsert(1) != ";")
            {
               break;
            }

            moveSelectionRelative();
            e.Handled = true;
            break;
         case '[':
            insertDelimiterText("[]", -1, 0);
            e.Handled = true;
            break;
         case ']':
            if (textAtInsert(1) != "]")
            {
               break;
            }

            moveSelectionRelative();
            e.Handled = true;
            break;
         case '{':
            insertDelimiterText("{}", -1, 0);
            e.Handled = true;
            break;
         case '}':
            if (textAtInsert(1) != "}")
            {
               break;
            }

            moveSelectionRelative();
            e.Handled = true;
            break;
      }
   }

   protected void textEditor_KeyUp(object sender, KeyEventArgs e)
   {
      switch (e.KeyCode)
      {
         case Keys.Escape:
            if (textAtInsert(1) == "'" && textAtInsert(1, -1) == "'" || textAtInsert(1) == "\"" && textAtInsert(1, -1) == "\"" ||
                textAtInsert(1) == ")" && textAtInsert(1, -1) == "(" || textAtInsert(1) == "]" && textAtInsert(1, -1) == "[")
            {
               e.Handled = true;
               setTextAtInsert(1);
               break;
            }

            if (textAtInsert(1) != "}" || textAtInsert(1, -1) != "{")
            {
               break;
            }

            e.Handled = true;
            setTextAtInsert(1);
            break;
         case Keys.F1:
            if (getWord() is (true, var begin) && findWord(begin) is (true, var source))
            {
               insertText(source.Drop(begin.Length), 0);
            }

            e.Handled = true;
            break;
      }
   }

   protected Maybe<string> getWord()
   {
      return textEditor.Text.Keep(textEditor.SelectionStart).Matches("/(/w+) $").Map(r => r.FirstGroup);
   }

   protected Maybe<string> findWord(string begin)
   {
      return textEditor.Text.Unjoin("-/w+").Distinct().Where(word => word.StartsWith(begin)).FirstOrNone();
   }

   protected void Playground_FormClosing(object sender, FormClosingEventArgs e)
   {
      if (document.FileName is (true, var fileName))
      {
         FileName file = fileName;
         if (file.Exists())
         {
            try
            {
               playgroundConfiguration.LastFile = file;
               playgroundConfiguration.Save();
            }
            catch (Exception exception)
            {
               MessageBox.Show(exception.DeepMessage());
            }
         }
      }
   }

   protected void Playground_KeyUp(object sender, KeyEventArgs e)
   {
      if (e.KeyCode == Keys.Escape)
      {
         context.Cancel();
         e.Handled = true;
      }
   }

   protected void textEditor_SelectionChanged(object sender, EventArgs e)
   {
      if (textEditor.FirstVisibleLine != firstEditorLine)
      {
         firstEditorLine = textEditor.FirstVisibleLine;
         textEditor.Invalidate();
      }
   }
}