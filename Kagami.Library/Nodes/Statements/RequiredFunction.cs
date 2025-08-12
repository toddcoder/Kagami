using Core.Monads;
using Kagami.Library.Inclusions;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using RequireFunction = Kagami.Library.Invokables.RequireFunction;

namespace Kagami.Library.Nodes.Statements;

public class RequiredFunction(Selector selector, Maybe<TypeConstraint> _typeConstraint, Inclusion inclusion) : Statement
{
   public Selector Selector => selector;

   public Maybe<TypeConstraint> TypeConstraint => _typeConstraint;

   public Inclusion Inclusion => inclusion;

   public override void Generate(OperationsBuilder builder)
   {
      var block = new Block(new OverrideOrThrow(selector));
      var invokable = new RequireFunction();
      var _index = builder.RegisterInvokable(invokable, block, false);
      if (_index)
      {
         var lambda = new Lambda(invokable, false);
         builder.PushObject(lambda);
         builder.AssignSelector(selector, false);
      }
   }

   public override string ToString() => $"required {selector}";
}