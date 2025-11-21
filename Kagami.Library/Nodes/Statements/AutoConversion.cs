using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Nodes.Statements;

public class AutoConversion(string parameterName, string fromClass, string toClass, Block block) : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      var @class = classOf(fromClass);
      var parameter = new Parameter(false, false, "", parameterName, nil, new TypeConstraint([@class]), false, false, false);
      var parameters = new Parameters(parameter);
      @class = classOf(toClass);
      block.TypeConstraint = new TypeConstraint([@class]);

      var lambdaSymbol = new LambdaSymbol(parameters, block);
      lambdaSymbol.Generate(builder);

      builder.RegisterAutoConversion(fromClass, toClass);
   }
}