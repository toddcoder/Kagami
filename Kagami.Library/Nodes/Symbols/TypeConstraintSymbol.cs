using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class TypeConstraintSymbol : Symbol, IConstant
{
   protected TypeConstraint typeConstraint;

   public TypeConstraintSymbol(IEnumerable<BaseClass> list) => typeConstraint = new TypeConstraint(list.ToArray());

   public override void Generate(OperationsBuilder builder)
   {
      typeConstraint.RefreshClasses();
      builder.PushObject(typeConstraint);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => typeConstraint.Image;

   public IObject Object => typeConstraint;
}