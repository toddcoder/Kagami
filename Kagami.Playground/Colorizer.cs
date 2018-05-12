using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Kagami.Library.Parsers;
using Standard.Types.RegularExpressions;
using Color = System.Drawing.Color;

namespace Kagami.Playground
{
   public class Colorizer
   {
      const int WM_SETREDRAW = 11;

      RichTextBox textBox;

      [DllImport("user32.dll")]
      public static extern int SendMessage(IntPtr hWnd, int msg, bool wParam, int lParam);

      public static void StopTextBoxUpdate(RichTextBox textBox) => SendMessage(textBox.Handle, WM_SETREDRAW, false, 0);

      public static void ResumeTextBoxUpdate(RichTextBox textBox) => SendMessage(textBox.Handle, WM_SETREDRAW, true, 0);

      public Colorizer(RichTextBox textBox) => this.textBox = textBox;

      public void Colorize(IEnumerable<Token> tokens)
      {
         var selectionStart = textBox.SelectionStart;
         var selectionLength = textBox.SelectionLength;
         var font = textBox.Font;
         using (var boldFont = new Font(textBox.Font, FontStyle.Bold))
         using (var italicFont = new Font(textBox.Font, FontStyle.Italic))
         {
            StopTextBoxUpdate(textBox);
            textBox.SelectAll();
            textBox.SelectionColor = Color.Black;
            textBox.SelectionBackColor = Color.White;
            foreach (var token in tokens)
            {
               textBox.Select(token.Index, token.Length);
               textBox.SelectionColor = getForeColor(token.Color);
               textBox.SelectionBackColor = getBackColor();
               textBox.SelectionFont = !isItalic(token.Color) ? (!isBold(token.Color) ? font : boldFont) : italicFont;
            }

            markText("/s+ (/r /n | /r | /n)", Color.PaleVioletRed);
            textBox.SelectionStart = selectionStart;
            textBox.SelectionLength = selectionLength;
            ResumeTextBoxUpdate(textBox);
            textBox.Refresh();
         }
      }

      void markText(string pattern, Color backColor)
      {
         if (textBox.Text.Matches(pattern).If(out var matcher))
            for (var matchIndex = 0; matchIndex < matcher.MatchCount; ++matchIndex)
            {
               var match = matcher.GetMatch(matchIndex);
               textBox.Select(match.Index, match.Length);
               textBox.SelectionBackColor = backColor;
            }
      }

      static bool isBold(Library.Parsers.Color color)
      {
         switch (color)
         {
            case Library.Parsers.Color.StringPart:
            case Library.Parsers.Color.NumberPart:
            case Library.Parsers.Color.Operator:
            case Library.Parsers.Color.Message:
            case Library.Parsers.Color.Collection:
            case Library.Parsers.Color.Keyword:
            case Library.Parsers.Color.Char:
               return true;
            default:
               return false;
         }
      }

      static bool isItalic(Library.Parsers.Color color) => color == Library.Parsers.Color.Identifier;

      static Color getBackColor() => Color.White;

      static Color getForeColor(Library.Parsers.Color color)
      {
         switch (color)
         {
            case Library.Parsers.Color.String:
            case Library.Parsers.Color.StringPart:
            case Library.Parsers.Color.Char:
               return Color.FromArgb(38, 205, 0);
            case Library.Parsers.Color.Number:
            case Library.Parsers.Color.NumberPart:
               return Color.Green;
            case Library.Parsers.Color.Operator:
               return Color.BlueViolet;
            case Library.Parsers.Color.Identifier:
               return Color.Blue;
            case Library.Parsers.Color.Structure:
               return Color.Black;
            case Library.Parsers.Color.Whitespace:
               return Color.Black;
            case Library.Parsers.Color.Comment:
               return Color.FromArgb(128, 128, 128);
            case Library.Parsers.Color.Message:
               return Color.Teal;
            case Library.Parsers.Color.Format:
               return Color.Violet;
            case Library.Parsers.Color.Date:
               return Color.DarkOliveGreen;
            case Library.Parsers.Color.Collection:
               return Color.LightCoral;
            case Library.Parsers.Color.Symbol:
               return Color.CornflowerBlue;
            case Library.Parsers.Color.Boolean:
               return Color.Coral;
            case Library.Parsers.Color.Keyword:
               return Color.Red;
            case Library.Parsers.Color.Invokable:
               return Color.DarkMagenta;
            case Library.Parsers.Color.Class:
               return Color.DarkGreen;
            case Library.Parsers.Color.Label:
               return Color.Gray;
            case Library.Parsers.Color.Type:
               return Color.DarkCyan;
            default:
               return Color.Black;
         }
      }
   }
}