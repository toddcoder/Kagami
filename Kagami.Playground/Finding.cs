namespace Kagami.Playground;

public abstract record Finding(string Text, bool IsRegex, bool IgnoreCase)
{
   public sealed record Find(string Text, bool IsRegex, bool IgnoreCase) : Finding(Text, IsRegex, IgnoreCase);

   public sealed record Replace(string Text, bool IsRegex, bool IgnoreCase, string Replacement) : Finding(Text, IsRegex, IgnoreCase);

   public sealed record ReplaceAll(string Text, bool IsRegex, bool IgnoreCase, string Replacement) : Finding(Text, IsRegex, IgnoreCase);
}