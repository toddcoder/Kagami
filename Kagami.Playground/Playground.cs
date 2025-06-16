using Core.Applications;
using Core.Arrays;
using Core.Collections;
using Core.Computers;
using Core.Dates;
using Core.Enumerables;
using Core.Exceptions;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using Core.WinForms;
using Core.WinForms.Consoles;
using Core.WinForms.Controls;
using Core.WinForms.Documents;
using Core.WinForms.TableLayoutPanels;
using Kagami.Library;
using Kagami.Library.Runtime;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Text;
using static Core.Monads.MonadFunctions;

namespace Kagami.Playground;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public partial class Playground : Form
{
   protected Document document = null!;
   protected TextBoxConsole outputConsole = null!;
   protected TextWriter textWriter = null!;
   protected TextReader textReader = null!;
   protected bool locked = true;
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
   protected Idle idle = new(1);
   protected bool isDirty;
   protected UiAction uiValue = new() { AutoSizeText = true };
   protected UiAction uiType = new() { AutoSizeText = true, LeftStripe = DashStyle.Dash };
   protected UiAction uiElapsed = new() { AutoSizeText = true };
   protected UiAction uiStatus = new();
   protected UiAction uiRun = new();

   public Playground()
   {
      UiAction.BusyStyle = BusyStyle.BarberPole;
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
         document = new Document(this, textEditor, ".kagami", "Kagami", playgroundConfiguration.FontName, playgroundConfiguration.FontSize,
            autoDirty: false);

         var menus = document.Menus;
         menus.Menu("&File");
         menus.Menu("&New", (_, _) =>
         {
            textEditor.ClearModificationGlyphs();
            document.New();
         });
         menus.Menu("&Open", (_, _) =>
         {
            textEditor.ClearModificationGlyphs();
            document.Open();
         }, "^O");
         menus.Menu("&Save", (_, _) =>
         {
            textEditor.SetToSavedGlyphs();
            document.Save();
         }, "^S");
         menus.Menu("Save As", (_, _) =>
         {
            textEditor.SetToSavedGlyphs();
            document.SaveAs();
         });
         menus.Menu("Exit", (_, _) => Close(), "%F4");

         document.StandardEditMenu();

         menus.Menu("&Edit");
         menus.Menu("Duplicate", (_, _) => duplicate(), "^D");
         menus.Menu("Create Block", (_, _) => createBlock(), "^B");
         menus.Menu("&Build");
         menus.Menu("Run", (_, _) => run(), "F5");
         menus.Menu("Manual", (s, _) =>
         {
            manual = !manual;
            ((ToolStripMenuItem)s!).Checked = manual;
         }, "^F5");
         menus.Menu("Dump operations", (s, _) =>
         {
            dumpOperations = !dumpOperations;
            ((ToolStripMenuItem)s!).Checked = dumpOperations;
         });
         menus.Menu("Trace", (s, _) =>
         {
            tracing = !tracing;
            ((ToolStripMenuItem)s!).Checked = tracing;
         }, "^T");
         menus.Menu("&Insert");
         menus.Menu("open sys", (_, _) => insertText("open sys\n\n", 0, 0), "^%S");
         menus.Menu("open math", (_, _) => insertText("open math\n\n", 0, 0), "^%M");
         menus.Menu("println()", (_, _) => insertText("println()", -1, 0), "^P");
         menus.Menu("println() interpolated", (_, _) => insertText("println($\"\")", -2, 0), "^%P");
         menus.Menu("print()", (_, _) => insertText("print()", -1, 0));
         menus.Menu("put()", (_, _) => insertText("put()", -1, 0));
         menus.Menu("peek()", (_, _) => surround("peek(", ")"), "^K");
         menus.Menu("Triple quotes", (_, _) => insertText("\"\"\"\n\"\"\"", -3), "^Q");
         menus.Menu("List", (_, _) => insertText("⌈⌉", -1), "^L");
         menus.Menu("Set", (_, _) => insertText("⎩⎭", -1), "^E");

         menus.Menu("&Debug");
         menus.Menu("Step Into", (_, _) => stepInto(), "F11");
         menus.Menu("Step Over", (_, _) => stepOver(), "F10");

         menus.CreateMainMenu(this);
         menus.StandardContextEdit(document);

         textEditor.SetTabs(32, 64, 96, 128);
         textEditor.SetLeftMargin(70);
         textEditor.ReassignHandle();
         textEditor.Paint += (_, evt) => paint(evt);
         textEditor.AnnotationFont = new Font(textEditor.Font, FontStyle.Bold);

         var contextMenu = new FreeMenus();
         contextMenu.StandardContextEdit();
         contextMenu.CreateContextMenu(textConsole);

         locked = false;
         manual = false;
         dumpOperations = false;
         tracing = false;
         stopwatch = new Stopwatch();
         _exceptionIndex = nil;
         cancelled = false;

         idle.Triggered.Handler = _ =>
         {
            try
            {
               _exceptionData = nil;
               if (!manual)
               {
                  textEditor.Do(() => update(!manual, false, false));
               }
            }
            catch
            {
            }
         };

         uiRun.Button("Run");
         uiRun.Click += (_, _) => run();
         uiRun.ClickText = "Run code";

         var builder = new TableLayoutBuilder(table);
         _ = builder.Col + 200 + 100f + 200;
         _ = builder.Row + 50f + 50f + 40 + 40;
         builder.SetUp();

         (builder + textEditor).SpanCol(3).Row();

         (builder + textConsole).SpanCol(3).Row();

         uiValue.ZeroOut();
         uiType.ZeroOut();
         (builder + uiValue).SpanCol(2).Next();
         (builder + uiType).Row();

         uiElapsed.ZeroOut();
         uiStatus.ZeroOut();
         uiRun.ZeroOut();
         (builder + uiElapsed).Next();
         (builder + uiStatus).Next();
         (builder + uiRun).Row();

         document.Open(playgroundConfiguration.LastFile);
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

   protected void run() => update(true, true, true);

   protected void update(bool execute, bool fromMenu, bool ignoreDirtyFlag)
   {
      if (!isDirty && !ignoreDirtyFlag)
      {
         return;
      }

      if (!locked && textEditor.TextLength != 0)
      {
         locked = true;
         if (manual)
         {
            uiStatus.Message("Running...");
            Application.DoEvents();
         }
         else if (fromMenu)
         {
            document.Save();
         }

         var status = (message: "Success", type: UiActionType.Success);
         try
         {
            uiValue.NoStatus("");
            uiStatus.Busy(true);
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
               var value = "not executed";
               var type = "";
               if (execute)
               {
                  var _result = machine.Execute();
                  if (_result is (true, var result))
                  {
                     cancelled = context.Cancelled();
                     context.Reset();
                     value = result.Image;
                     type = result.ClassName;
                  }
                  else
                  {
                     _exceptionIndex = compiler.ExceptionIndex;
                     if (!_exceptionIndex)
                     {
                        _exceptionIndex = textEditor.SelectionStart;
                     }

                     value = "exception";
                     type = "";
                     status = (message: _result.Exception.Message, type: UiActionType.Failure);
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
                  var text = operations.ToString();
                  textWriter.WriteLine(text);
                  if (document.FileName is (true, var fileName))
                  {
                     FileName documentFile = fileName;
                     var dumpFile = documentFile.Folder + $"{documentFile.Name}.txt";
                     dumpFile.TryTo.SetText(text, Encoding.UTF8);
                  }
               }

               document.Clean();

               uiElapsed.Message(stopwatch.Elapsed.ToLongString(true));
               if (cancelled)
               {
                  uiValue.Failure("cancelled");
                  uiType.Failure("");
                  cancelled = false;
               }
               else if (value == "exception")
               {
                  uiValue.Failure(value);
                  uiType.Failure("");
               }
               else
               {
                  uiValue.Success(value);
                  uiType.Success(type);
               }
            }
            else
            {
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

               status = (message: _machine.Exception.Message, type: UiActionType.Failure);
            }
         }
         catch (Exception exception)
         {
            status = (exception.Message, UiActionType.Failure);
         }
         finally
         {
            locked = false;
            isDirty = false;
            uiStatus.ShowMessage(status.message, status.type);
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
      _exceptionData = new ExceptionData(index, length);
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
            if (peeks.Maybe[lineNumber] is (true, var str))
            {
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
      if (locked)
      {
         isDirty = false;
         document.Clean();
      }
      else
      {
         isDirty = true;
         document.Dirty();
         uiStatus.Message("changed...");
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
         case '\'':
            if (textAtInsert(1) == "'")
            {
               moveSelectionRelative();
               e.Handled = true;
               break;
            }

            insertDelimiterText("''", -1, 0);
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

   protected void timerIdle_Tick(object sender, EventArgs e) => idle.CheckIdleTime();

   protected void textEditor_KeyDown(object sender, KeyEventArgs e)
   {
      if (e.KeyCode == Keys.Back)
      {
         deleteMatching('(', ')');
         deleteMatching('[', ']');
         deleteMatching('{', '}');
         deleteMatching('\'', '\'');
         deleteMatching('"', '"');
      }

      return;

      void deleteMatching(char left, char right)
      {
         var caretPosition = textEditor.SelectionStart;
         if (caretPosition > 0 && textEditor.TextLength > 0)
         {
            if (textEditor.Text[caretPosition - 1] == left && caretPosition < textEditor.TextLength && textEditor.Text[caretPosition] == right)
            {
               textEditor.Text = textEditor.Text.Remove(caretPosition - 1, 2);
               textEditor.SelectionStart = caretPosition - 1;
               e.SuppressKeyPress = true;
            }
         }
      }
   }
}