using Core.Collections;
using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Parsers.Statements;

public class AbstractClassBuilder : ClassBuilder
{
   protected Hash<Selector, AbstractFunction> abstractFunctions;

   public AbstractClassBuilder(string className, Parameters parameters, string parentClassName, Expression[] parentArguments, bool initialize,
      Block constructorBlock, Hash<Selector, AbstractFunction> abstractFunctions) : base(className, parameters, parentClassName, parentArguments,
      initialize, constructorBlock)
   {
      this.abstractFunctions = abstractFunctions;
   }

   public override UserClass CreateUserClass() => new(className, parentClassName);

   protected override void preHandleStatement(Statement statement)
   {
      switch (statement)
      {
         case Function function:
         {
            var (selector, _, block, _, _, _) = function;
            if (abstractFunctions.Maybe[selector] is (true, var abstractFunction) &&
                matchingTypeConstraints(abstractFunction.TypeConstraint, block.TypeConstraint))
            {
               abstractFunctions.Maybe[selector] = nil;
            }

            break;
         }
         case AssignToNewField assignToNewField:
         {
            var (mutable, fieldName, _typeConstraint) = assignToNewField;
            var getter = Function.Getter(fieldName);
            var (selector, _, _, _, _, _) = getter;
            if (abstractFunctions.Maybe[selector] is (true, var abstractFunction1) &&
                matchingTypeConstraints(abstractFunction1.TypeConstraint, _typeConstraint))
            {
               abstractFunctions.Maybe[selector] = nil;
            }

            if (mutable)
            {
               var setter = Function.Setter(fieldName);
               (selector, _, _, _, _, _) = setter;
               if (abstractFunctions.Maybe[selector] is (true, var abstractFunction2) &&
                   matchingTypeConstraints(abstractFunction2.TypeConstraint, _typeConstraint))
               {
                  abstractFunctions.Maybe[selector] = nil;
               }
            }

            break;
         }
      }
   }
}