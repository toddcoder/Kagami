using Standard.Applications;
using Standard.WinForms.Controls;

namespace Kagami.Playground
{
   public static class ExRichTextBoxExtensions
   {
      public static (int start, int length) StopAutoscrollingAlways(this ExRichTextBox textBox)
      {
         textBox.StopUpdating();
         var (start, length) = textBox.SaveSelection();
         if (textBox.Focused)
            textBox.Parent.Focus();
         User32.SendMessage(textBox.Handle, User32.Messages.HideSelection, true, 0);

         return (start, length);
      }

      public static void ResumeAutoscrollingAlways(this ExRichTextBox textBox, (int start, int length) state)
      {
         textBox.RestoreSelection(state.start, state.length);
         User32.SendMessage(textBox.Handle, User32.Messages.HideSelection, false, 0);

         textBox.ResumeUpdating();
         textBox.Refresh();

         if (!textBox.Focused)
            textBox.Focus();
      }
   }
}