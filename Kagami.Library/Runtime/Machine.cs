using System;
using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Packages;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using Standard.Types.Exceptions;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Maybe.AttemptFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Runtime
{
   public class Machine
   {
      const int MAX_DEPTH = 128;

      public static Machine Current { get; set; }

      public static Fields Fields => Current.CurrentFrame.Fields;

      IContext context;
      Stack<Frame> stack;
      Operations.Operations operations;
      bool running;
      Lazy<TableMaker> table;
      DebugState debugState;
      GlobalFrame globalFrame;

      public Machine(IContext context)
      {
         this.context = context;
         stack = new Stack<Frame>();
         operations = new Operations.Operations();
         running = false;
         table = new Lazy<TableMaker>(() =>
            new TableMaker(("Address", Justification.Left), ("Operation", Justification.Left), ("Stack", Justification.Left)));
         debugState = DebugState.Starting;
         globalFrame = new GlobalFrame();
      }

      public IContext Context => context;

      public Operations.Operations Operations => operations;

      public void Load(Operations.Operations operations) => this.operations = operations;

      void trace(int address, Func<string> message)
      {
         if (Tracing)
            table.Value.Add(address.ToString("D5"), message().Truncate(80), StackImage.Truncate(80));
      }

      public GlobalFrame GlobalFrame => globalFrame;

      public IResult<Unit> Execute()
      {
         stack.Clear();
         globalFrame = new GlobalFrame();
         stack.Push(globalFrame);
         operations.Goto(0);
         running = true;

         while (!context.Cancelled() && operations.More && running)
            if (operations.Current.If(out var operation))
            {
               trace(operations.Address, () => operation.ToString());
               var currentAddress = operations.Address;
               if (operation.Execute(this).If(out var result, out var original) && running && result.ClassName != "Void")
                  stack.Peek().Push(result);
               else if (original.IsFailedMatch)
               {
                  if (Tracing)
                     context.PrintLine(table.Value.ToString());
                  return failure<Unit>(original.Exception);
               }

               if (operation.Increment && currentAddress == operations.Address)
                  operations.Advance(1);
            }
            else
               return failure<Unit>(addressOutOfRange());

         if (Tracing)
            context.PrintLine(table.Value.ToString());

         return Unit.Success();
      }

      public IMatched<IObject> Invoke(IInvokable invokable, Arguments arguments, Fields fields, int increment = 1)
      {
         var returnAddress = Address + increment;
         var frame = new Frame(returnAddress, fields);

         if (invokable is YieldingInvokable yfi)
            yfi.Arguments = arguments;

         PushFrame(frame);
         frame = new Frame(arguments);
         PushFrame(frame);
         frame.SetFields(invokable.Parameters);
         if (GoTo(invokable.Address))
            return invoke();
         else
            return failedMatch<IObject>(badAddress(invokable.Address));
      }

      public IMatched<IObject> Invoke(IInvokable invokable, Arguments arguments, int increment = 1)
      {
         var returnAddress = Address + increment;
         var frame = new Frame(returnAddress, arguments);

         if (invokable is YieldingInvokable yfi)
            yfi.Arguments = arguments;

         PushFrame(frame);
         frame.SetFields(invokable.Parameters);
         if (GoTo(invokable.Address))
            return invoke();
         else
            return failedMatch<IObject>(badAddress(invokable.Address));
      }

      public IMatched<IObject> Invoke(YieldingInvokable invokable)
      {
         var frames = invokable.Frames;
         if (frames.FunctionFrame.If(out var frame))
         {
            frame.Address = Address;
            PushFrames(frames);
         }
         else
         {
            frame = new Frame(Address, invokable.Arguments);
            PushFrame(frame);
         }

         frame.SetFields(invokable.Parameters);
         GoTo(invokable.Address);

         return invoke();
      }

      public IMatched<IObject> Invoke(string fieldName)
      {
         if (Pop().If(out var value, out var exception))
            if (value is Arguments arguments)
            {
               var selector = arguments.Selector(fieldName);
               if (Find(selector).If(out var field, out var isNotMatched, out exception))
               {
                  value = field.Value;
                  switch (value)
                  {
                     case IInvokableObject io:
                        return Invoke(io.Invokable, arguments);
                     case IInvokable invokable:
                        return Invoke(invokable, arguments);
                     case PackageFunction pf:
                        return pf.Invoke(arguments).Matched();
                     default:
                        return failedMatch<IObject>(incompatibleClasses(value, "Invokable"));
                  }
               }
               else if (isNotMatched)
                  return failedMatch<IObject>(fieldNotFound(selector));
               else
                  return failedMatch<IObject>(exception);
            }
            else
               return failedMatch<IObject>(incompatibleClasses(value, "Arguments"));
         else
            return failedMatch<IObject>(exception);
      }

      IMatched<IObject> invoke()
      {
         while (!context.Cancelled() && operations.More && running)
            if (operations.Current.If(out var operation))
            {
               trace(operations.Address, () => operation.ToString());
               var currentAddress = operations.Address;
               switch (operation)
               {
                  case Return rtn:
                     return Return.ReturnAction(this, rtn.ReturnTopOfStack);
                  case Yield _:
                     return Yield.YieldAction(this);
                  case Invoke invoke:
                     if (Invoke(invoke.FieldName).If(out var returnValue, out var isNotMatched, out var exception))
                        stack.Peek().Push(returnValue);
                     else if (!isNotMatched)
                        return failedMatch<IObject>(exception);

                     if (currentAddress == operations.Address)
                        operations.Advance(1);
                     continue;
               }

               if (operation.Execute(this).If(out var result, out var original) && running)
                  stack.Peek().Push(result);
               else if (original.IsFailedMatch)
                  return failedMatch<IObject>(original.Exception);

               if (operation.Increment && currentAddress == operations.Address)
                  operations.Advance(1);
            }
            else
               return "Address out of range".FailedMatch<IObject>();

         return "No return".FailedMatch<IObject>();
      }

      public bool Running
      {
         get => running;
         set => running = value;
      }

      public void Push(IObject value) => stack.Peek().Push(value);

      public IMaybe<IObject> Peek() => stack.Peek().Peek();

      public IResult<IObject> Pop() => stack.Peek().Pop();

      public bool IsEmpty => stack.Peek().IsEmpty;

      public bool GoTo(int address) => operations.Goto(address);

      public void Advance(int increment) => operations.Advance(increment);

      public bool Tracing { get; set; }

      public int Address => operations.Address;

      public Frame CurrentFrame => stack.Peek();

      public void PushFrame(Frame frame)
      {
         stack.Push(frame);
         if (stack.Count > MAX_DEPTH)
            throw "Max stack depth".Throws();
      }

      public void PushFrames(FrameGroup frames)
      {
         foreach (var frame in frames.Reverse())
            PushFrame(frame);
      }

      public IResult<Frame> PopFrame() => tryTo(() => stack.Pop());

      public FrameGroup PopFrames() => PopFramesUntil(f => f.FrameType == FrameType.Function);

      public FrameGroup PeekFrames(Predicate<Frame> predicate)
      {
         var frames = new List<Frame>();
         foreach (var frame in stack)
         {
            frames.Add(frame);
            if (predicate(frame))
               break;
         }

         return new FrameGroup(frames.ToArray());
      }

      public IMaybe<Frame> FunctionFrame()
      {
         foreach (var frame in stack)
            if (frame.FrameType == FrameType.Function)
               return frame.Some();

         return none<Frame>();
      }

      public FrameGroup PopFramesUntil(Predicate<Frame> predicate)
      {
         var frames = new List<Frame>();
         while (stack.Count > 0)
         {
            var frame = stack.Pop();
            frames.Add(frame);
            if (predicate(frame))
               break;
         }

         return new FrameGroup(frames.ToArray());
      }

      public void Clear() => CurrentFrame.Clear();

      public string StackImage => stack.Select(f => f.ToString()).Listify();

      public IObject X { get; set; } = Unassigned.Value;

      public IObject Y { get; set; } = Unassigned.Value;

      public IObject Z { get; set; } = Unassigned.Value;

      public IMatched<Field> Find(string fieldName, bool getting)
      {
         var depth = 0;
         foreach (var frame in stack)
            if (frame.Fields.Find(fieldName, getting).If(out var field, out var isNotMatched, out var exception))
               return field.Matched();
            else if (isNotMatched)
            {
               if (depth++ > MAX_DEPTH)
                  return "Max depth exceeded".FailedMatch<Field>();
            }
            else
               return failedMatch<Field>(exception);

         return notMatched<Field>();
      }

      public IResult<Unit> FindByPattern(string pattern, List<Field> list)
      {
         foreach (var frame in stack)
            if (frame.Fields.FindByPattern(pattern, list).IfNot(out var exception))
               return failure<Unit>(exception);

         return Unit.Success();
      }

      public IMatched<Field> Find(Selector selector) => findExact(selector).Or(findEquivalent(selector).Or(findTypeless(selector)));

      IMatched<Field> findExact(Selector selector) => Find(selector.Image, true);

      IMatched<Field> findEquivalent(Selector selector)
      {
         var count = selector.SelectorItems.Length;
         var iterator = new BitIterator(count);
         foreach (var bools in iterator)
         {
            var newSelector = selector.Equivalent(bools);
            if (findExact(newSelector).If(out var matched, out var isNotMatched, out var exception))
               return matched.Matched();
            else if (!isNotMatched)
               return failedMatch<Field>(exception);
         }

         return notMatched<Field>();
      }

      IMatched<Field> findTypeless(Selector selector) => Find(selector.LabelsOnly().Image, true);

      public IResult<Field> Assign(string fieldName, IObject value, bool getting, bool overriden = false)
      {
         if (Find(fieldName, getting).If(out var field, out var isNotMatched, out var exception))
            if (field.Mutable)
            {
               var baseClass = classOf(value);
               if (field.Value is Unassigned || field.Value is TypeConstraint tc && tc.Matches(baseClass) ||
                  classOf(field.Value).AssignCompatible(baseClass))
               {
                  if (field.Value is Reference r)
                     r.Field.Value = value;
                  else
                     field.Value = value;
                  return field.Success();
               }
               else if (field.Value is TypeConstraint tc2)
                  return failure<Field>(incompatibleClasses(value, tc2.AsString));
               else
                  return failure<Field>(incompatibleClasses(value, field.Value.ClassName));
            }
            else if (field.Value is Unassigned || overriden)
            {
               if (field.Value is Reference r)
                  r.Field.Value = value;
               else
                  field.Value = value;
               return field.Success();
            }
            else
               return failure<Field>(immutableField(fieldName));
         else if (isNotMatched)
            return failure<Field>(fieldNotFound(fieldName));
         else
            return failure<Field>(exception);
      }

      public void BeginDebugging()
      {
         debugState = DebugState.Active;
         stack.Clear();
         stack.Push(new Frame());
         operations.Goto(0);
      }

      public IResult<DebugState> Step(bool into, Hash<string, IObject> watch)
      {
         if (debugState == DebugState.Starting)
            BeginDebugging();

         while (!context.Cancelled() && operations.More && debugState == DebugState.Active)
            if (operations.Current.If(out var operation))
            {
               var currentAddress = operations.Address;
               switch (operation)
               {
                  case Break _:
                     return debugState.Success();
                  default:
                     if (operation.Execute(this).If(out var result, out var original) && running && result.ClassName != "Void")
                        stack.Peek().Push(result);
                     else if (original.IsFailedMatch)
                        return failure<DebugState>(original.Exception);

                     break;
               }

               if (operation.Increment && currentAddress == operations.Address)
                  operations.Advance(1);
            }
            else
               return failure<DebugState>(addressOutOfRange());

         return debugState.Success();
      }

      public string PackageFolder { get; set; } = "";
   }
}