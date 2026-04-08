using Core.Enumerables;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Packages;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class Invoke : OneOperandOperation
{
   public static void InvokeInvokableObject(Machine machine, IInvokableObject invokableObject, Arguments arguments)
   {
      var invokable = invokableObject.Invokable;
      if (invokable is YieldingInvokable yi)
      {
         InvokeYieldingInvokable(machine, yi, arguments);
      }
      else
      {
         Maybe<Fields> _fields = nil;
         if (invokableObject is IProvidesFields providesFields)
         {
            _fields = providesFields.Fields;
         }

         InvokeInvokable(machine, invokable, arguments, _fields, (invokableObject as Lambda).NotNull());
      }
   }

   protected static void InvokeYieldingInvokable(Machine machine, YieldingInvokable invokable, Arguments arguments)
   {
      invokable.Arguments = arguments;
      var iterator = invokable.GetIterator(true);
      machine.Push((IObject)iterator);
   }

   public static void InvokeInvokable(Machine machine, IInvokable invokable, Arguments arguments, Maybe<Fields> _fields, Maybe<Lambda> _lambda)
   {
      if (invokable.Constructing)
      {
         InvokeConstructor(machine, invokable, arguments);
      }
      else
      {
         var frame = new Frame(invokable.RequiresFunctionFrame ? machine.Address : nil, arguments);
         machine.PushFrame(frame);
         frame.SetFields(invokable.Parameters);
         if (_fields is (true, var fields))
         {
            if (!fields.ContainsKey("this") && _lambda is (true, var lambda))
            {
               fields.New("this", FieldType.Assignment, lambda).Force();
            }

            frame.SetFields(fields);
         }

         frame.Lambda = _lambda;
         machine.GoTo(invokable.Address);
      }
   }

   public static void InvokeConstructor(Machine machine, IInvokable invokable, Arguments arguments)
   {
      var frame = new Frame(invokable.RequiresFunctionFrame ? machine.Address : nil, arguments);
      machine.PushFrame(frame);
      frame.SetFields(invokable.Parameters);
      frame.CurrentClass = invokable.Class;
      machine.GoTo(invokable.Address);
   }

   public static Optional<IObject> InvokeObject(Machine machine, IObject value, Arguments arguments, ref bool increment)
   {
      switch (value)
      {
         case CompositeLambda compositeLambda:
            increment = true;
            return compositeLambda.Invoke(arguments.Value).Just();
         case IInvokableObject invokableObject:
            InvokeInvokableObject(machine, invokableObject, arguments);
            increment = invokableObject.Invokable is YieldingInvokable;
            return nil;
         case PackageFunction packageFunction:
            increment = true;
            var result = packageFunction.Invoke(arguments);
            return result.Just();
         case IMayInvoke mayInvoke:
            increment = true;
            return mayInvoke.Invoke(arguments.Value).Just();
         case Pattern pattern:
            increment = true;
            var copy = pattern.Copy();
            copy.RegisterArguments(arguments);
            return copy;
         case UserObject userObject:
            increment = true;
            return sendMessage(userObject, "invoke(_...)", arguments).Just();
         default:
            return incompatibleClasses(value, "Invokable object");
      }
   }

   protected string fieldName;
   protected bool increment;

   public Invoke(string fieldName)
   {
      this.fieldName = fieldName;
   }

   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      try
      {
         increment = false;
         if (value is Arguments arguments)
         {
            if (fieldName == "this" && machine.PeekFramesUntil(f => f.Lambda).LastOrNone() is (true, { Lambda: (true, var lambda) }))
            {
               return InvokeObject(machine, lambda, arguments, ref increment);
            }

            var _field = machine.Find(fieldName, true);
            if (_field is (true, var foundField))
            {
               return InvokeObject(machine, foundField.Value, arguments, ref increment);
            }
            else
            {
               var selector = arguments.Selector(fieldName);
               var image = selector.Image;
               _field = machine.Find(selector);
               if (_field is (true, var selectedField))
               {
                  return InvokeObject(machine, selectedField.Value, arguments, ref increment);
               }
               else if (_field.Exception is (true, var exception))
               {
                  return exception;
               }
               else
               {
                  return fieldNotFound(image);
               }
            }
         }
         else
         {
            return incompatibleClasses(value, "Arguments");
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public override bool Increment => increment;

   public string FieldName => fieldName;

   public override string ToString() => $"invoke({fieldName})";
}