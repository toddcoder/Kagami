using Core.Applications;
using Core.WinForms.Controls;

namespace Kagami.Playground
{
	public static class ExRichTextBoxExtensions
	{
		public static (int start, int length) StopAutoScrollingAlways(this ExRichTextBox textBox)
		{
			textBox.StopUpdating();
			var (start, length) = textBox.Selection;
			if (textBox.Focused)
				textBox.Parent.Focus();
			User32.SendMessage(textBox.Handle, User32.Messages.HideSelection, true, 0);

			return (start, length);
		}

		public static void ResumeAutoScrollingAlways(this ExRichTextBox textBox, (int start, int length) state)
		{
			textBox.Selection = state;
			User32.SendMessage(textBox.Handle, User32.Messages.HideSelection, false, 0);

			textBox.ResumeUpdating();
			textBox.Refresh();

			if (!textBox.Focused)
				textBox.Focus();
		}
	}
}