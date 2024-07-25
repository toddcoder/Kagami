namespace Kagami.Library.Parsers
{
   public class Token
   {
      public Token(int index, int length, string text)
      {
         Index = index;
         Length = length;
         Text = text;
      }

      public int Index { get; }

      public int Length { get; }

      public string Text { get; }

      public Color Color { get; set; }
   }
}