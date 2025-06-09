using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class OptionalFunction(Selector selector) : Statement
{
   public Selector Selector => selector;
   public override void Generate(OperationsBuilder builder)
   {
   }
}