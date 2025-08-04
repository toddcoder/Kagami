namespace Kagami.Library.Parsers.Expressions;

public abstract record LazyStringPart(string Text)
{
   public sealed record String(string Text) : LazyStringPart(Text);

   public sealed record Field(string Text) : LazyStringPart(Text);

   public sealed record Format(string Text) : LazyStringPart(Text);

   public sealed record Hex(string Text) : LazyStringPart(Text);
}