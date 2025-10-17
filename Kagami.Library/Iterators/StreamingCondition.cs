using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public abstract record StreamingCondition
{
   public sealed record Continuing(IObject Item) : StreamingCondition;

   public sealed record Finished : StreamingCondition;

   public sealed record Failed(string Message) : StreamingCondition;

   public sealed record Skipping : StreamingCondition;

   public sealed record Terminated(IObject Item) : StreamingCondition;
}