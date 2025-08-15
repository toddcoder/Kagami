using Core.Applications.Messaging;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;
using static Core.Monads.MonadFunctions;

namespace Kagami.Playground;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public partial class FindReplace : Form
{
   public const string FIND_REPLACE = "find-replace";
   public const string REPLACE_ALL = "replace-all";
   public const string REPLACE_SOURCE = "replace-source";

   protected LabelText ltFind = new("Find");
   protected LabelText ltReplace = new("Replace");
   protected UiAction uiRegex = new();
   protected UiAction uiIgnoreCase = new();
   protected UiAction uiFind = new();
   protected UiAction uiReplace = new();
   protected UiAction uiReplaceAll = new();
   protected UiAction uiMessage = new();
   protected ReplacementSource replacementSource = new("", 0, 0);
   protected Subscriber<ReplacementSource> replacementSourceSubscriber = new(REPLACE_SOURCE);

   public FindReplace()
   {
      InitializeComponent();

      uiRegex.CheckBox("Regex", true);

      uiFind.Button("Find");
      uiFind.Click += (_, _) =>
      {
         var selection = replacementSource.Selection;
         var _newSelection = find(uiRegex.BoxChecked, uiIgnoreCase.BoxChecked, selection.index, selection.length);
         if (_newSelection is (true, var (index, length)))
         {
            Publisher<FindReplacement>.Publish(FIND_REPLACE, "find", new FindReplacement(index, length, ""));
            uiMessage.Success("Found");
         }
         else
         {
            uiMessage.Failure("Not found");
         }
      };

      uiReplace.Click += (_, _) =>
      {
         var selection = replacementSource.Selection;
         var _newSelection = find(uiRegex.BoxChecked, uiIgnoreCase.BoxChecked, selection.index, selection.length);
         if (_newSelection is (true, var (index, length)))
         {
            Publisher<FindReplacement>.Publish(FIND_REPLACE, "replace", new FindReplacement(index, length, ltReplace.Text));
            uiMessage.Success("Replaced");
         }
         else
         {
            uiMessage.Failure("Not replaced");
         }
      };

      uiReplaceAll.Click += (_, _) =>
      {
         FindReplacement[] findReplacements =
            [.. findAll(uiRegex.BoxChecked, uiIgnoreCase.BoxChecked).Select(t => new FindReplacement(t.index, t.length, ltReplace.Text))];
         /*Slicer slicer = this.playground.Editor.Text;
         var replacement = ltReplace.Text;
         var count = 0;
         foreach (var (index, length) in findAll(uiRegex.BoxChecked, uiIgnoreCase.BoxChecked))
         {
            slicer[index, length] = replacement;
            count++;
         }

         var selection = this.playground.Editor.Selection;
         this.playground.Editor.Text = slicer.ToString();
         this.playground.Editor.Selection = selection;

         if (count > 0)
         {
            uiMessage.Success(count.Plural("item(s) replaced"));
         }
         else
         {
            uiMessage.Failure("Not replaced");
         }*/
      };

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col * 5 * 20f;
      _ = builder.Row * 4 * 50;
      builder.SetUp();

      (builder + ltFind).SpanCol(5).Row();
      (builder + ltReplace).SpanCol(5).Row();
      (builder + uiRegex).Next();
      (builder + uiIgnoreCase).Next();
      (builder + uiFind).Next();
      (builder + uiReplace).Next();
      (builder + uiReplaceAll).Row();
      (builder + uiMessage).SpanCol(5).Row();

      replacementSourceSubscriber["activated"] = p => replacementSource = p.Payload;
   }

   protected Maybe<(int index, int length)> find(bool useRegex, bool ignoreCase, int startIndex, int startLength)
   {
      var offset = startIndex + startLength;
      if (useRegex)
      {
         var searchText = replacementSource.Text.Drop(offset);

         Pattern pattern = ltFind.Text;
         if (ignoreCase)
         {
            pattern = pattern.WithIgnoreCase(true);
         }

         if (searchText.Matches(pattern) is (true, var result))
         {
            var match = result.Matches[0];
            return (match.Index + offset, match.Length);
         }
         else
         {
            return nil;
         }
      }
      else
      {
         var _index = replacementSource.Text.Find(ltFind.Text, offset, ignoreCase);
         return _index.Map(i => (i, ltFind.Text.Length));
      }
   }

   protected IEnumerable<(int index, int length)> findAll(bool useRegex, bool ignoreCase)
   {
      var startIndex = 0;
      var startLength = 0;
      while (find(useRegex, ignoreCase, startIndex, startLength) is (true, var selection))
      {
         startIndex = selection.index;
         startLength = selection.length;

         yield return selection;
      }
   }
}