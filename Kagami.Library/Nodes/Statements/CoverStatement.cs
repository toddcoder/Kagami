using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class CoverStatement(Expression expression, string identifier, Maybe<Block> _first, Maybe<Block> _middle, Maybe<Block> _last) : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      var parameter = Parameter.New(false, false, identifier);
      var parameters = new Parameters(parameter);
      expression.Generate(builder);
      builder.NewCover();

      if (_first is (true, var first))
      {
         var lambdaSymbol = new LambdaSymbol(parameters, first);
         lambdaSymbol.Generate(builder);
         builder.SendMessage("first(_<Lambda>)", 1);
      }

      if (_middle is (true, var middle))
      {
         var lambdaSymbol = new LambdaSymbol(parameters, middle);
         lambdaSymbol.Generate(builder);
         builder.SendMessage("middle(_<Lambda>)", 1);
      }

      if (_last is (true, var last))
      {
         var lambdaSymbol = new LambdaSymbol(parameters, last);
         lambdaSymbol.Generate(builder);
         builder.SendMessage("last(_<Lambda>)", 1);
      }

      builder.Iterate();
   }
}