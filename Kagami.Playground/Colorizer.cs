using System.Runtime.InteropServices;
using Kagami.Library.Parsers;
using Core.Matching;
using Color = System.Drawing.Color;

namespace Kagami.Playground;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public class Colorizer
{
   protected RichTextBox textBox;
   protected int parenthesesCount;

   [DllImport("user32.dll")]
   public static extern int SendMessage(IntPtr hWnd, int msg, bool wParam, int lParam);

   public Colorizer(RichTextBox textBox)
   {
      this.textBox = textBox;
      parenthesesCount = 0;
   }

   public void Colorize(IEnumerable<Token> tokens)
   {
      parenthesesCount = 0;
      var font = textBox.Font;
      using var boldFont = new Font(textBox.Font, FontStyle.Bold);
      textBox.SelectAll();
      textBox.SelectionColor = Color.Black;
      textBox.SelectionBackColor = Color.White;
      foreach (var token in tokens)
      {
         textBox.Select(token.Index, token.Length);
         textBox.SelectionColor = getForeColor(token.Color, ref parenthesesCount);
         textBox.SelectionBackColor = getBackColor(token.Color);
         textBox.SelectionFont = isBold(token.Color) ? boldFont : font;
      }

      markText("/s+ (/r /n | /r | /n)", Color.BlanchedAlmond);
   }

   protected void markText(string pattern, Color backColor)
   {
      if (textBox.Text.Matches(pattern) is (true, var result))
      {
         for (var matchIndex = 0; matchIndex < result.MatchCount; ++matchIndex)
         {
            var (_, index, length) = result.GetMatch(matchIndex);
            textBox.Select(index, length);
            textBox.SelectionBackColor = backColor;
         }
      }
   }

   protected static bool isBold(Library.Parsers.Color color) => color switch
   {
      Library.Parsers.Color.StringPart => true,
      Library.Parsers.Color.NumberPart => true,
      Library.Parsers.Color.Operator => true,
      Library.Parsers.Color.Message => true,
      Library.Parsers.Color.Collection => true,
      Library.Parsers.Color.CollectionPart => true,
      Library.Parsers.Color.Keyword => true,
      Library.Parsers.Color.Char => true,
      Library.Parsers.Color.Selector => true,
      Library.Parsers.Color.Regex => true,
      _ => false
   };

   protected static Color getBackColor(Library.Parsers.Color color) => color switch
   {
      Library.Parsers.Color.OpenParenthesis => SystemColors.Info,
      Library.Parsers.Color.CloseParenthesis => SystemColors.Info,
      _ => Color.White
   };

   protected static Color getParenthesisColor(Library.Parsers.Color color, ref int parenthesesCount) => color switch
   {
      Library.Parsers.Color.OpenParenthesis => getParenthesisColor(++parenthesesCount),
      Library.Parsers.Color.CloseParenthesis => getParenthesisColor(parenthesesCount--),
      _ => Color.Black
   };

   protected static Color getParenthesisColor(int parenthesesCount) => parenthesesCount switch
   {
      1 => Color.Black,
      2 => Color.Blue,
      3 => Color.Green,
      4 => Color.DarkCyan,
      5 => Color.YellowGreen,
      6 => Color.Gray,
      _ => Color.Red
   };

   protected static Color getForeColor(Library.Parsers.Color color, ref int parenthesesCount) => color switch
   {
      Library.Parsers.Color.String => Color.FromArgb(38, 205, 0),
      Library.Parsers.Color.StringPart => Color.FromArgb(38, 205, 0),
      Library.Parsers.Color.Char => Color.FromArgb(38, 205, 0),
      Library.Parsers.Color.Number => Color.Green,
      Library.Parsers.Color.NumberPart => Color.Green,
      Library.Parsers.Color.Operator => Color.BlueViolet,
      Library.Parsers.Color.Identifier => Color.Blue,
      Library.Parsers.Color.Structure => Color.Black,
      Library.Parsers.Color.Whitespace => Color.Black,
      Library.Parsers.Color.Comment => Color.FromArgb(128, 128, 128),
      Library.Parsers.Color.Message => Color.Teal,
      Library.Parsers.Color.Format => Color.Violet,
      Library.Parsers.Color.Date => Color.DarkOliveGreen,
      Library.Parsers.Color.Collection => Color.Purple,
      Library.Parsers.Color.CollectionPart => Color.Purple,
      Library.Parsers.Color.Symbol => Color.CornflowerBlue,
      Library.Parsers.Color.Boolean => Color.Coral,
      Library.Parsers.Color.Keyword => Color.Red,
      Library.Parsers.Color.Invokable => Color.DarkMagenta,
      Library.Parsers.Color.Class => Color.DarkGreen,
      Library.Parsers.Color.Label => Color.DimGray,
      Library.Parsers.Color.Type => Color.DarkCyan,
      Library.Parsers.Color.OpenParenthesis => getParenthesisColor(color, ref parenthesesCount),
      Library.Parsers.Color.CloseParenthesis => getParenthesisColor(color, ref parenthesesCount),
      Library.Parsers.Color.Selector => Color.CadetBlue,
      Library.Parsers.Color.Regex => Color.OrangeRed,
      _ => Color.Black
   };
}