using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class IteratorSymbol : Symbol
{
   protected bool lazy;
   protected bool indexed;

   public IteratorSymbol(bool lazy, bool indexed)
   {
      this.lazy = lazy;
      this.indexed = indexed;
   }

   public override void Generate(OperationsBuilder builder)
   {
      if (indexed)
      {
         builder.SendMessage("indexed()");
      }
      else
      {
         builder.GetIterator(lazy);
      }
   }

   public override Precedence Precedence => Precedence.PrefixOperator;

   public override Arity Arity => Arity.Prefix;

   public override string ToString() => indexed ? "?" : lazy ? "!!" : "!";
}