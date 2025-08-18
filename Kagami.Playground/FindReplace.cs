using Core.Applications.Messaging;
using Core.WinForms;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Kagami.Playground;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public partial class FindReplace : Form
{
   protected LabelText ltFind = new("Find");
   protected LabelText ltReplace = new("Replace");
   protected UiAction uiRegex = new();
   protected UiAction uiIgnoreCase = new();
   protected UiAction uiFind = new();
   protected UiAction uiReplace = new();
   protected UiAction uiReplaceAll = new();
   protected UiAction uiMessage = new();
   protected Subscriber<string> subscriber = new("find-replace");

   public FindReplace()
   {
      InitializeComponent();

      uiRegex.CheckBox("Regex", false);
      uiIgnoreCase.CheckBox("Ignore Case", false);

      uiFind.Button("Find");
      uiFind.Click += (_, _) =>
      {
         var payload = new Finding.Find(ltFind.Text, uiRegex.BoxChecked, uiIgnoreCase.BoxChecked);
         Publisher<Finding>.Publish("finding", "find", payload);
      };
      uiFind.ClickText = "Find text in editor";

      uiReplace.Button("Replace");
      uiReplace.Click += (_, _) =>
      {
         var payload = new Finding.Replace(ltFind.Text, uiRegex.BoxChecked, uiIgnoreCase.BoxChecked, ltReplace.Text);
         Publisher<Finding>.Publish("finding", "replace", payload);
      };
      uiReplace.ClickText = "Replace text in editor";

      uiReplaceAll.Button("Replace All");
      uiReplaceAll.Click += (_, _) =>
      {
         var payload = new Finding.ReplaceAll(ltFind.Text, uiRegex.BoxChecked, uiIgnoreCase.BoxChecked, ltReplace.Text);
         Publisher<Finding>.Publish("finding", "replace-all", payload);
      };

      uiMessage.Message("");

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col * 5 * 20f;
      _ = builder.Row * 4 * 50 + 100f;
      builder.SetUp();

      (builder + ltFind).SpanCol(5).Row();
      (builder + ltReplace).SpanCol(5).Row();
      (builder + uiRegex).Next();
      (builder + uiIgnoreCase).Next();
      (builder + uiFind).Next();
      (builder + uiReplace).Next();
      (builder + uiReplaceAll).Row();
      (builder + uiMessage).SpanCol(5).Row();

      this.Tuck(uiMessage);

      subscriber["success"] = p => uiMessage.Do(() => uiMessage.Success(p.Payload));
      subscriber["message"] = p => uiMessage.Do(() => uiMessage.Message(p.Payload));
      subscriber["failure"] = p => uiMessage.Do(() => uiMessage.Failure(p.Payload));
      subscriber.UnsubscribeOnClose(this);
   }
}