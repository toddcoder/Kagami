using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Runtime;
using Core.Booleans;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Nodes.Statements;

public class Function : Statement, IOverridable, IAnnotatable
{
   public static Function Getter(string fieldName, Maybe<TypeConstraint> _typeConstraint)
   {
      return new($"__${fieldName}", new Parameters(0), false, Block.Getter(fieldName, _typeConstraint), false, false, "") { IsGetter = true };
   }

   public static Function Getter(string fieldName) => Getter(fieldName, nil);

   public static Function Getter(string getterName, string fieldName, Maybe<TypeConstraint> _typeConstraint)
   {
      return new(getterName, new Parameters(0), false, Block.Getter(fieldName, _typeConstraint), false, false, "") { IsGetter = true };
   }

   public static Function Getter(string getterName, string fieldName) => Getter(getterName, fieldName, nil);

   public static Function Setter(string fieldName, Maybe<TypeConstraint> _typeConstraint)
   {
      var parameters = new Parameters(1);
      return new Function($"{fieldName}=", parameters, false, Block.Setter(fieldName, parameters[0].Name, _typeConstraint), false, false, "")
         { IsSetter = true };
   }

   public static Function Setter(string fieldName) => Setter(fieldName, nil);

   public static Function Setter(string setterName, string fieldName, Maybe<TypeConstraint> _typeConstraint)
   {
      var parameters = new Parameters(1);
      return new Function(setterName, parameters, false, Block.Setter(fieldName, parameters[0].Name, _typeConstraint), false, false, "")
         { IsSetter = true };
   }

   public static Function Setter(string setterName, string fieldName) => Setter(setterName, fieldName, nil);

   protected Selector selector;
   protected Parameters parameters;
   protected bool isHidden;
   protected Block block;
   protected bool yielding;
   protected bool overriding;
   protected string className;
   protected Lazy<Lambda> lambda;

   public Function(string functionName, Parameters parameters, bool isHidden, Block block, bool yielding, bool overriding, string className,
      bool captures = false)
   {
      selector = parameters.Selector(functionName);
      this.parameters = parameters;
      this.isHidden = isHidden;
      this.block = block;
      this.yielding = yielding;
      block.Yielding = this.yielding;
      this.overriding = overriding;
      this.className = className;

      lambda = new Lazy<Lambda>(() =>
      {
         var invokable = GetInvokable();
         return new Lambda(invokable, captures);
      });
   }

   public void Deconstruct(out Selector selector, out Parameters parameters, out Block block, out bool yielding,
      out IInvokable invokable, out bool overriding, out bool isHidden)
   {
      selector = this.selector;
      parameters = this.parameters;
      block = this.block;
      yielding = this.yielding;
      invokable = GetInvokable();
      overriding = this.overriding;
      isHidden = this.isHidden;
   }

   public Selector Selector => selector;

   public Parameters Parameters => parameters;

   public Block Block => block;

   public string ClassName
   {
      get => className;
      set => className = value;
   }

   public Maybe<Class> Class { get; set; } = nil;

   public bool Yielding => yielding;

   public bool Overriding => overriding;

   public IInvokable Invokable => lambda.Value.Invokable;

   public IInvokable GetInvokable()
   {
      if (yielding)
      {
         return new YieldingInvokable(selector, parameters, ToString());
      }
      else
      {
         return new FunctionInvokable(selector, parameters, ToString());
      }
   }

   public override void Generate(OperationsBuilder builder)
   {
      var _index = builder.RegisterInvokable(lambda.Value.Invokable, block, overriding);
      if (_index)
      {
         if (parameters.Length > 0 && parameters[^1].Variadic)
         {
            var lambdaName = selector.Name;
            lambdaName = className.IsNotEmpty() ? $"{className}.{lambdaName}" : lambdaName;
            if (!overriding)
            {
               builder.NewField(lambdaName, false, true);
            }

            builder.PushObject(lambda.Value);
            builder.LambdaCapture();
            builder.AssignField(lambdaName, overriding);
         }
         else
         {
            string fullFunctionName = selector;
            _ = className.IsNotEmpty() ? $"{className}.{fullFunctionName}" : fullFunctionName;
            if (!overriding)
            {
               builder.NewSelector(selector, false, true);
            }

            builder.PushObject(lambda.Value);
            builder.LambdaCapture();
            builder.AssignSelector(selector, overriding);
         }

         builder.ProcessAnnotations(this);
      }
      else
      {
         throw _index.Exception;
      }

      if (className.IsNotEmpty())
      {
         if (Module.Global.Value.Class(className) is (true, var cls))
         {
            cls.RegisterMessage(selector, (obj, msg) => BaseClass.Invoke(obj, msg.Arguments, lambda.Value, false));
         }
         else
         {
            throw classNotFound(className);
         }
      }
   }

   public override string ToString()
   {
      return $"{overriding.Extend("override ")}{yielding.Extend("co")}func {selector.Image} ...";
   }

   public void SetToOverriding() => overriding = true;

   public bool IsFixed { get; set; }

   public bool IsGetter { get; set; }

   public bool IsSetter { get; set; }

   public bool IsAbstract { get; set; }

   public Lambda Lambda => lambda.Value;

   public void Fix() => IsFixed = true;
}