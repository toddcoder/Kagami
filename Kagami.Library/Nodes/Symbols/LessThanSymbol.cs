using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class 
   LessThanSymbol : Symbol, IPrefixCode
{
   protected bool prefix;

   public override void Generate(OperationsBuilder builder)
   {
      if (prefix)
      {
         builder.Dup();
         builder.SwapAt(1);
      }

      builder.Compare();
      builder.IsNegative();
   }

   public override Precedence Precedence => Precedence.Boolean;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "<";

   public void Prefix()
   {
      prefix = true;
   }
}