using Kagami.Library.Objects;
using Kagami.Library.Operations;
using RequireFunction = Kagami.Library.Invokables.RequireFunction;

namespace Kagami.Library.Nodes.Statements;

public class RequiredFunction(Selector selector) : Statement
{
   public Selector Selector => selector;

   public override void Generate(OperationsBuilder builder)
   {
      var block = new Block(new OverrideOrThrow(selector));
      var invokable = new RequireFunction();
      var _index = builder.RegisterInvokable(invokable, block, false);
      if (_index)
      {
         var lambda = new Lambda(invokable);
         builder.PushObject(lambda);
         builder.Peek(Index);
         builder.AssignSelector(selector, false);
      }
   }

   public override string ToString() => $"required {selector}";
}