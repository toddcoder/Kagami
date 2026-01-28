using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;

namespace Kagami.Library.Nodes.Statements;

public interface IAnnotatable
{
   Selector Selector { get; }

   Lambda Lambda { get; }

   List<InvokeSymbol> Annotations { get; }
}