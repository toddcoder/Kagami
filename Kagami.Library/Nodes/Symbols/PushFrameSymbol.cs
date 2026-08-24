using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class PushFrameSymbol(bool withValue = false) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      if (withValue)
      {
         builder.PushFrameWithValue();
      }
      else
      {
         builder.PushFrame();
      }
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;
}