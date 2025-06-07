using Core.Collections;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Inclusions;

public class Inclusion(string name)
{
   protected Hash<Selector, RequiredFunction> requiredFunctions = [];
   protected Hash<Selector, OptionalFunction> optionalFunctions = [];
   protected Hash<Selector, Function> functions = [];
   protected StringHash<Inclusion> inheritedInclusions = [];

   public string Name => name;

   public Result<Unit> Register(RequiredFunction requiredFunction)
   {
      if (requiredFunctions.ContainsKey(requiredFunction.Selector))
      {
         return fail($"Required function {requiredFunction.Selector} already exists");
      }
      else
      {
         requiredFunctions[requiredFunction.Selector] = requiredFunction;
         return unit;
      }
   }

   public Result<Unit> Register(OptionalFunction optionalFunction)
   {
      if (optionalFunctions.ContainsKey(optionalFunction.Selector))
      {
         return fail($"Optional function {optionalFunction.Selector} already exists");
      }
      else
      {
         optionalFunctions[optionalFunction.Selector] = optionalFunction;
         return unit;
      }
   }

   public Result<Unit> Register(Function function)
   {
      if (functions.ContainsKey(function.Selector))
      {
         return fail($"Function {function.Selector} already exists");
      }
      else
      {
         functions[function.Selector] = function;
         return unit;
      }
   }

   public Result<Unit> Register(Inclusion inclusion)
   {
      if (inheritedInclusions.ContainsKey(inclusion.Name))
      {
         return fail($"Inclusion {inclusion.Name} already exists");
      }
      else
      {
         inheritedInclusions[inclusion.Name] = inclusion;
         return unit;
      }
   }

   public Maybe<RequiredFunction> RequiredFunction(Selector selector) => requiredFunctions.Maybe[selector];

   public Maybe<OptionalFunction> OptionalFunction(Selector selector) => optionalFunctions.Maybe[selector];

   public Maybe<Function> Function(Selector selector) => functions.Maybe[selector];

   public Maybe<Inclusion> InheritedInclusion(string inclusionName) => inheritedInclusions.Maybe[inclusionName];
}