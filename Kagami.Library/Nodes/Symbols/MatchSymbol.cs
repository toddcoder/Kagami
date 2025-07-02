using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class MatchSymbol(bool not) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.Match();
      if (not)
      {
         builder.PushBoolean(false);
         builder.Equal();
      }
   }

   public override Precedence Precedence => Precedence.Boolean;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "|=";
}