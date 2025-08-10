namespace Kagami.Library.Parsers.Expressions;

public abstract record StringPart(string Text)
{
   public sealed record String(string Text) : StringPart(Text);

   public sealed record Field(string Text) : StringPart(Text);

   public sealed record Format(string Text) : StringPart(Text);

   public sealed record Hex(string Text) : StringPart(Text);
}