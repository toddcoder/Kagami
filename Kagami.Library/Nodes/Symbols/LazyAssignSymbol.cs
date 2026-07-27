using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class LazyAssignSymbol(Expression expression) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      var fieldName = newLabel("field");
      var lazyName = NodeFunctions.lazyName(fieldName);
      var block = new Block(expression);
      block.AddReturnIf();
      var invokable = new LambdaInvokable(Parameters.Empty, block.ToString());
      var _index = builder.RegisterInvokable(invokable, block, true);
      if (_index)
      {
         builder.NewLambda(invokable, true);
         builder.StoreField(lazyName, false, true, false, nil);
         builder.PushObject(new Objects.Singleton());
         builder.StoreField(fieldName, true, true, false, nil);
         builder.PushObject(new SymbolObject(lazyName));
      }
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;
}