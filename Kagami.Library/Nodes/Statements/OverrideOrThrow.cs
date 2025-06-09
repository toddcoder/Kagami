using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class OverrideOrThrow(Selector selector) : Statement
{
   public override void Generate(OperationsBuilder builder) => builder.RequireFunction(selector);
}