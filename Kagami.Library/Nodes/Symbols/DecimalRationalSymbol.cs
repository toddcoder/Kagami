using System.Numerics;
using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class DecimalRationalSymbol(BigInteger numerator, BigInteger denominator) : Symbol, IConstant
{
   protected Rational rational = new(numerator, denominator);

   public override void Generate(OperationsBuilder builder) => builder.PushObject(rational);

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"{numerator}|{denominator}";

   public IObject Object => rational;
}