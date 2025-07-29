using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class IteratorSymbol : Symbol
{
   protected bool lazy;
   protected bool indexed;
   protected bool range;

   public IteratorSymbol(bool lazy, bool indexed, bool range)
   {
      this.lazy = lazy;
      this.indexed = indexed;
      this.range = range;
   }

   public override void Generate(OperationsBuilder builder)
   {
      if (indexed)
      {
         builder.SendMessage("indexed()");
      }
      else if (lazy)
      {
         builder.GetIterator(lazy);
      }
      else if (range)
      {
         builder.SendMessage("range", 0);
      }
      else
      {
         builder.GetIterator(false);
      }
   }

   public override Precedence Precedence => Precedence.TightPrefixOperator;

   public override Arity Arity => Arity.Prefix;

   public override string ToString()
   {
      if (indexed)
      {
         return "iit";
      }
      else if (lazy)
      {
         return "lit";
      }
      else if (range)
      {
         return "rng";
      }
      else
      {
         return "it";
      }
   }
}