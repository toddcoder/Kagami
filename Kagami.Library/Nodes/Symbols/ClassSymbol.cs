using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class ClassSymbol : Symbol, IConstant
   {
      string className;

      public ClassSymbol(string className) => this.className = className;

      public override void Generate(OperationsBuilder builder) => builder.PushObject(new Class(className));

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => className;

      public IObject Object => new Class(className);
   }
}