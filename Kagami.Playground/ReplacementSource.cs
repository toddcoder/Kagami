namespace Kagami.Playground;

public readonly struct ReplacementSource(string text, int index, int length)
{
   public string Text => text;

   public int Index => index;

   public int Length => length;

   public (int index, int length) Selection => (index, length);
}