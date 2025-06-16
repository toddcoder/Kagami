using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class ObjectSymbol : Symbol
{
   protected IObject obj;

   public ObjectSymbol(IObject obj) => this.obj = obj;

   public override void Generate(OperationsBuilder builder) => builder.PushObject(obj);

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => obj.Image;
}