using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Arrays;
using Kagami.Library;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Computers;
using Core.Collections;
using Core.Dates;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using Core.ObjectGraphs;
using Core.RegularExpressions;
using Core.Strings;
using Core.WinForms.Consoles;
using Core.WinForms.Documents;
using static Core.Arrays.ArrayFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Playground
{
   public partial class Playground : Form
   {
      protected const string PLAYGROUND_FONT_NAME = "Consolas";
      protected const float PLAYGROUND_FONT_SIZE = 14f;
      protected const string CONFIGURATION_FOLDER = @":\Configurations\Kagami\";
      protected const string PLAYGROUND_CONFIGURATION_FILE = CONFIGURATION_FOLDER + "Kagami.configuration";
      protected const string PLAYGROUND_PACKAGE_FOLDER = CONFIGURATION_FOLDER + "Packages";
      protected const string KAGAMI_EXCEPTION_PROMPT = "Kagami exception >>> ";

      protected Document document;
      protected TextBoxConsole outputConsole;
      protected TextWriter textWriter;
      protected TextReader textReader;
      protected bool locked;
      protected bool manual;
      protected Stopwatch stopwatch;
      protected PlaygroundConfiguration playgroundConfiguration;
      protected PlaygroundContext context;
      protected Colorizer colorizer;
      protected bool dumpOperations;
      protected bool tracing;
      protected IMaybe<int> _exceptionIndex;
      protected bool cancelled;
      protected Hash<string, IObject> watch;
      protected FolderName packageFolder;
      protected IMaybe<ExceptionData> _exceptionData;
      protected int firstEditorLine;

      public Playground()
      {
         InitializeComponent();
         firstEditorLine = 0;
      }

      protected static IResult<FileName> existingConfigurationFile()
      {
         foreach (var drive in array('C', 'E'))
         {
            FileName configurationFile = $"{drive}{PLAYGROUND_CONFIGURATION_FILE}";
            if (configurationFile.Exists())
            {
               return configurationFile.Success();
            }
         }

         return "Couldn't find configuration file".Failure<FileName>();
      }

      protected static IResult<PlaygroundConfiguration> getConfiguration(FileName configurationFile) =>
         from objectGraph in ObjectGraph.Try.FromFile(configurationFile)
         from configuration in objectGraph.Object<PlaygroundConfiguration>()
         select configuration;

      protected static IResult<Unit> setConfiguration(PlaygroundConfiguration configuration, FileName configurationFile) =>
         from serialized in ObjectGraph.Try.Serialize(configuration)
         from unit in configurationFile.TryTo.SetText(serialized.ToString())
         select unit;

      protected void Playground_Load(object sender, EventArgs e)
      {
         try
         {
            var result =
               from file in existingConfigurationFile()
               from config in getConfiguration(file)
               select config;
            if (result.If(out playgroundConfiguration))
            {
            }
            else
            {
               playgroundConfiguration = new PlaygroundConfiguration
               {
                  DefaultFolder = FolderName.Current,
                  FontName = PLAYGROUND_FONT_NAME,
                  FontSize = PLAYGROUND_FONT_SIZE,
                  PackageFolder = "C" + PLAYGROUND_PACKAGE_FOLDER
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
            _exceptionData = none<ExceptionData>();
            document = new Document(this, textEditor, ".kagami", "Kagami", playgroundConfiguration.FontName, playgroundConfiguration.FontSize);
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
            menus.Menu("Edit", "Indent", (_, _) => indent(), "^I");
            menus.Menu("Edit", "Unindent", (_, _) => unindent(), "^%I");
            menus.Menu("&Build");
            menus.Menu("Build", "Run", (_, _) => run(), "F5");
            menus.Menu("Build", "Manual", (s, _) =>
            {
               manual = !manual;
               ((ToolStripMenuItem)s).Checked = manual;
            }, "^F5");
            menus.Menu("Build", "Dump operations", (s, _) =>
            {
               dumpOperations = !dumpOperations;
               ((ToolStripMenuItem)s).Checked = dumpOperations;
            });
            menus.Menu("Build", "Trace", (s, _) =>
            {
               tracing = !tracing;
               ((ToolStripMenuItem)s).Checked = tracing;
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
            _exceptionIndex = none<int>();
            cancelled = false;
            watch = new Hash<string, IObject>();
            if (playgroundConfiguration.LastFile != null)
            {
               document.Open(playgroundConfiguration.LastFile);
            }
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
               _exceptionIndex = none<int>();
               _exceptionData = none<ExceptionData>();

               var kagamiConfiguration = new CompilerConfiguration { ShowOperations = dumpOperations, Tracing = tracing };
               var compiler = new Compiler(textEditor.Text, kagamiConfiguration, context);
               if (compiler.Generate().If(out var machine, out var exception))
               {
                  machine.PackageFolder = packageFolder.FullPath;
                  if (execute)
                  {
                     if (machine.Execute().IfNot(out exception))
                     {
                        textWriter.WriteLine(KAGAMI_EXCEPTION_PROMPT + exception.Message);
                        _exceptionIndex = compiler.ExceptionIndex;
                        if (_exceptionIndex.IsNone)
                        {
                           _exceptionIndex = textEditor.SelectionStart.Some();
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

                  if (_exceptionIndex.If(out var index))
                  {
                     var remainingLineLength = getRemainingLineIndex(index);
                     if (remainingLineLength > -1)
                     {
                        showException(index, remainingLineLength);
                     }
                  }

                  if (dumpOperations && compiler.Operations.If(out var operations))
                  {
                     textWriter.WriteLine(operations);
                  }

                  document.Clean();

                  labelStatus.Text = stopwatch.Elapsed.ToLongString(true) + (cancelled ? " (cancelled)" : "");
               }
               else
               {
                  textWriter.WriteLine(KAGAMI_EXCEPTION_PROMPT + exception.Message);
                  _exceptionIndex = compiler.ExceptionIndex;
                  if (_exceptionIndex.IsNone)
                  {
                     _exceptionIndex = textEditor.SelectionStart.Some();
                  }

                  if (_exceptionIndex.If(out var index))
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

      protected static void stepInto() => Machine.Current?.Step();

      protected static void stepOver() => Machine.Current?.Step();

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
               text = lines[number].Copy();
            }
         }
         else
         {
            text = textEditor.SelectedText;
         }

         textEditor.AppendAtEnd(text, "/n");
      }

      protected static void indent()
      {
      }

      protected static void unindent()
      {
      }

      protected IMaybe<(string[] lines, int index)> linesFromSelection()
      {
         var lineFromCharIndex1 = textEditor.GetLineFromCharIndex(textEditor.SelectionStart);
         var lineFromCharIndex2 = textEditor.GetLineFromCharIndex(textEditor.SelectionStart + textEditor.SelectionLength);
         var lines = textEditor.Lines;
         if (lineFromCharIndex1.Between(0).Until(lines.Length) && lineFromCharIndex2.Between(0).Until(lines.Length))
         {
            return (lines.RangeOf(lineFromCharIndex1, lineFromCharIndex2), lineFromCharIndex1).Some();
         }

         return none<(string[], int)>();
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

               if (_exceptionData.If(out var exceptionData))
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
            if (annotationWidth <= remainingWidth)
            {
               return annotation;
            }
            else
            {
               while (annotationWidth > remainingWidth)
               {
                  annotation = annotation.Drop(-2) + dots;
                  annotationWidth = getWidth(graphics, annotation, annotationFont);
               }

               return annotation;
            }
         }
      }

      protected void textEditor_TextChanged(object sender, EventArgs e)
      {
         try
         {
            _exceptionData = none<ExceptionData>();

            if (document != null)
            {
               update(!manual, false);
               document.Dirty();
            }
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
               {
                  break;
               }

               e.Handled = true;
               setTextAtInsert(1);
               break;
            case Keys.F1:
               if (getWord().If(out var begin) && findWord(begin).If(out var source))
               {
                  insertText(source.Drop(begin.Length), 0);
               }

               e.Handled = true;
               break;
         }
      }

      protected IMaybe<string> getWord()
      {
         return textEditor.Text.Keep(textEditor.SelectionStart).Matcher("/(/w+) $").Map(m => m.FirstGroup);
      }

      protected IMaybe<string> findWord(string begin)
      {
         return textEditor.Text.Split("-/w+").Distinct().Where(word => word.StartsWith(begin)).FirstOrNone();
      }

      protected void Playground_FormClosing(object sender, FormClosingEventArgs e)
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
}