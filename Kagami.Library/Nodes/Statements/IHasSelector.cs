using Kagami.Library.Objects;

namespace Kagami.Library.Nodes.Statements;

public interface IHasSelector
{
   Selector Selector { get; }

   bool Overriding { get; }
}