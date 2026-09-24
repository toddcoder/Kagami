using Core.Monads;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class JunctionSymbol2(string type) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      Maybe<string> _junctionType = type switch
      {
         "&" => "All",
         "|" => "Any",
         "^" => "One",
         "!" => "None",
         "@" => "Mappable",
         _ => nil
      };
      if (_junctionType is (true, var junctionType))
      {
         builder.NewJunction(junctionType);
      }
      else
      {
         throw new Exception($"Invalid junction type: {type}");
      }
   }

   public override Precedence Precedence => Precedence.TightPrefixOperator;

   public override Arity Arity => Arity.Prefix;

   public override string ToString() => $"{type}:";
}