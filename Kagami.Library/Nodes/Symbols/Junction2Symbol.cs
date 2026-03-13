using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class Junction2Symbol(JunctionType junctionType) : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.NewJunction2(junctionType);

   public override Precedence Precedence => Precedence.Comma;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => junctionType.OperatorString;
}