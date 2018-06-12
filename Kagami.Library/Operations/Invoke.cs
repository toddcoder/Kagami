using System;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Packages;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class Invoke : OneOperandOperation
   {
      public static void InvokeInvokableObject(Machine machine, IInvokableObject invokableObject, Arguments arguments)
      {
         var invokable = invokableObject.Invokable;
         if (invokable is YieldingInvokable yi)
            InvokeYieldingInvokable(machine, yi, arguments);
         else
            InvokeInvokable(machine, invokable, arguments,
               invokableObject is IProvidesFields pf && pf.ProvidesFields ? pf.Fields : new Fields());
      }

      static void InvokeYieldingInvokable(Machine machine, YieldingInvokable invokable, Arguments arguments)
      {
         invokable.Arguments = arguments;
         var iterator = invokable.GetIterator(false);
         machine.Push((IObject)iterator);
      }

      public static void InvokeInvokable(Machine machine, IInvokable invokable, Arguments arguments, Fields fields)
      {
         if (invokable.Constructing)
            InvokeConstructor(machine, invokable, arguments, fields);
         else
         {
            var returnAddress = machine.Address + 1;
            var frame = new Frame(returnAddress, fields);
            machine.PushFrame(frame);
            frame = new Frame(arguments);
            frame.SetFields(invokable.Parameters);
            machine.PushFrame(frame);
            machine.GoTo(invokable.Address);
         }
      }

      public static void InvokeConstructor(Machine machine, IInvokable invokable, Arguments arguments, Fields fields)
      {
         var returnAddress = machine.Address + 1;
         var frame = new Frame(returnAddress, arguments, fields);
         machine.PushFrame(frame);
         frame.SetFields(invokable.Parameters);
         machine.GoTo(invokable.Address);
      }

      string fieldName;
      bool increment;

      public Invoke(string fieldName) => this.fieldName = fieldName;

      public override IMatched<IObject> Execute(Machine machine, IObject value)
      {
         increment = false;
         if (value is Arguments arguments)
         {
            var fullFieldName = arguments.FullFunctionName(fieldName);
            var (type, field, exception) = machine.Find(fullFieldName, true).Values;
            switch (type)
            {
               case MatchType.Matched:
                  switch (field.Value)
                  {
                     case IInvokableObject io:
                        InvokeInvokableObject(machine, io, arguments);
                        increment = io.Invokable is YieldingInvokable;

                        return notMatched<IObject>();
                     case PackageFunction pf:
                        increment = true;
                        return pf.Invoke(arguments).Matched();
                     case IMayInvoke mi:
                        increment = true;
                        return mi.Invoke(arguments.Value).Matched();
                     default:
                        return failedMatch<IObject>(incompatibleClasses(field.Value, "Invokable object"));
                  }
               case MatchType.NotMatched:
                  return failedMatch<IObject>(fieldNotFound(fieldName));
               case MatchType.FailedMatch:
                  return failedMatch<IObject>(exception);
               default:
                  throw new ArgumentOutOfRangeException();
            }
         }
         else
            return failedMatch<IObject>(incompatibleClasses(value, "Arguments"));
      }

      public override bool Increment => increment;

      public string FieldName => fieldName;

      public override string ToString() => $"invoke({fieldName})";
   }
}