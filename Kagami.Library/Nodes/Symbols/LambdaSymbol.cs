using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class LambdaSymbol : Symbol
{
   protected Parameters parameters;
   protected Block block;
   protected bool captures;

   public LambdaSymbol(Parameters parameters, Block block, bool addReturnUnit = false, bool captures = true)
   {
      this.parameters = parameters;
      this.block = block;
      if (addReturnUnit)
      {
         this.block.AddReturnUnitIf();
      }

      this.captures = captures;
   }

   public LambdaSymbol(Parameters parameters, Expression expression, bool captures = true)
   {
      this.parameters = parameters;
      block = (Block)expression;
      this.captures = captures;
   }

   public LambdaSymbol(int unknownFieldCount, Expression expression, bool captures = true)
   {
      parameters = [with(unknownFieldCount)];
      block = (Block)expression;
      this.captures = captures;
   }

   public LambdaSymbol(int unknownFieldCount, Block block, bool captures = true)
   {
      parameters = [with(unknownFieldCount)];
      this.block = block;
      this.captures = captures;
   }

   public override void Generate(OperationsBuilder builder)
   {
      var invokable = new LambdaInvokable(parameters, ToString());
      var _index = builder.RegisterInvokable(invokable, block, true);
      if (_index)
      {
         builder.NewLambda(invokable, captures);
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