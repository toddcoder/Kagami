using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class LambdaSymbol : Symbol
{
   protected Parameters parameters;
   protected Block block;

   public LambdaSymbol(Parameters parameters, Block block)
   {
      this.parameters = parameters;
      this.block = block;
   }

   public LambdaSymbol(Parameters parameters, Expression expression)
   {
      this.parameters = parameters;
      block = (Block)expression;
   }

   public LambdaSymbol(int unknownFieldCount, Expression expression)
   {
      parameters = new Parameters(unknownFieldCount);
      block = (Block)expression;
   }

   public LambdaSymbol(int unknownFieldCount, Block block)
   {
      parameters = new Parameters(unknownFieldCount);
      this.block = block;
   }

   public override void Generate(OperationsBuilder builder)
   {
      var invokable = new LambdaInvokable(parameters, ToString());
      var _index = builder.RegisterInvokable(invokable, block, true);
      if (_index)
      {
         builder.NewLambda(invokable, true);
      }
      else
      {
         throw _index.Exception;
      }
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"({parameters}) -> {block}";
}