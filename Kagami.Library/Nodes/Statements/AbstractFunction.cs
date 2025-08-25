using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Nodes.Statements;

public class AbstractFunction(string functionName, Parameters parameters, Maybe<TypeConstraint> _typeConstraint)
   : Function(functionName, parameters, new Block(), false, false, ""), IEquatable<AbstractFunction>
{
   protected Maybe<TypeConstraint> _typeConstraint = _typeConstraint;

   public override void Generate(OperationsBuilder builder)
   {
   }

   public void Deconstruct(out Selector selector, out Parameters parameters, out Maybe<TypeConstraint> _typeConstraint)
   {
      selector = this.selector;
      parameters = this.parameters;
      _typeConstraint = this._typeConstraint;
   }

   public bool Equals(AbstractFunction? other)
   {
      return other is not null && other.selector.Equals(selector) && other.parameters.Equals(parameters) &&
         matchingTypeConstraints(other._typeConstraint, _typeConstraint);
   }

   public override bool Equals(object? obj) => obj is AbstractFunction otherAbstractFunction && Equals(otherAbstractFunction);

   public override int GetHashCode() => HashCode.Combine(selector, parameters, _typeConstraint);

   public static bool operator ==(AbstractFunction? left, AbstractFunction? right) => Equals(left, right);

   public static bool operator !=(AbstractFunction? left, AbstractFunction? right) => !Equals(left, right);

   public bool Matches(Function function)
   {
      if (selector != function.Selector)
      {
         return false;
      }

      if (_typeConstraint is (true, var typeConstraint))
      {
         var _functionTypeConstraint = function.Block.TypeConstraint;
         if (_functionTypeConstraint is (true, var functionTypeConstraint))
         {
            if (!Equals(typeConstraint, functionTypeConstraint))
            {
               return false;
            }
         }
      }

      return true;
   }
}