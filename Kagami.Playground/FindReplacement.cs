namespace Kagami.Playground;

public readonly struct FindReplacement(int index, int length, string replacement)
{
   public int Index => index;

   public int Length => length;

   public string Replacement => replacement;
}