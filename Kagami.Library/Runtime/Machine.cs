﻿using System;
using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Packages;
using Core.Enumerables;
using Core.Exceptions;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.AllExceptions;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;
using Failure = Kagami.Library.Objects.Failure;

namespace Kagami.Library.Runtime
{
   public class Machine
   {
      protected const int MAX_DEPTH = 128;

      public static Machine Current { get; set; }

      public static Fields Fields => Current.CurrentFrame.Fields;

      protected IContext context;
      protected Stack<Frame> stack;
      protected Operations.Operations operations;
      protected bool running;
      protected Lazy<TableMaker> table;
      protected DebugState debugState;
      protected GlobalFrame globalFrame;

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

      protected void trace(int address, Func<string> message)
      {
         if (Tracing)
         {
            table.Value.Add(address.ToString("D5"), message().Truncate(80), StackImage.Truncate(80));
         }
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
         {
            if (operations.Current.If(out var operation))
            {
               trace(operations.Address, () => operation.ToString());
               var currentAddress = operations.Address;
               if (operation.Execute(this).ValueOrOriginal(out var result, out var original) && running && result.ClassName != "Void")
               {
                  stack.Peek().Push(result);
               }
               else if (original.IfNot(out var anyException) && anyException.If(out var exception))
               {
                  if (Tracing)
                  {
                     context.PrintLine(table.Value.ToString());
                  }

                  if (GetErrorHandler().If(out var address))
                  {
                     stack.Peek().Push(new Failure(exception.Message));
                     operations.Goto(address);
                  }
                  else
                  {
                     return failure<Unit>(exception);
                  }
               }

               if (operation.Increment && currentAddress == operations.Address)
               {
                  operations.Advance(1);
               }
            }
            else
            {
               return failure<Unit>(addressOutOfRange());
            }
         }

         if (Tracing)
         {
            context.PrintLine(table.Value.ToString());
         }

         return Unit.Success();
      }

      public IMatched<IObject> Invoke(IInvokable invokable, Arguments arguments, Fields fields, int increment = 1)
      {
         var returnAddress = Address + increment;
         var frame = new Frame(returnAddress, fields);

         if (invokable is YieldingInvokable yfi)
         {
            yfi.Arguments = arguments;
         }

         PushFrame(frame);
         frame = new Frame(arguments);
         PushFrame(frame);
         frame.SetFields(invokable.Parameters);
         if (GoTo(invokable.Address))
         {
            var result = invoke();
            return result;
         }
         else
         {
            return failedMatch<IObject>(badAddress(invokable.Address));
         }
      }

      public IMatched<IObject> Invoke(IInvokable invokable, Arguments arguments, int increment = 1)
      {
         var returnAddress = Address + increment;
         var frame = new Frame(returnAddress, arguments);

         if (invokable is YieldingInvokable yfi)
         {
            yfi.Arguments = arguments;
         }

         PushFrame(frame);
         frame.SetFields(invokable.Parameters);

         return GoTo(invokable.Address) ? invoke() : failedMatch<IObject>(badAddress(invokable.Address));
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
         if (Pop().If(out var value, out var popException))
         {
            if (value is Arguments arguments)
            {
               var image = fieldName;
               var ((isFound, field), (isFailure, exception)) = Find(fieldName, true);
               if (!isFound && !isFailure)
               {
                  var selector = arguments.Selector(fieldName);
                  image = selector.Image;
                  ((isFound, field), (isFailure, exception)) = Find(selector);
               }

               if (isFound)
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
                     case Pattern pattern:
                        var copy = pattern.Copy();
                        copy.RegisterArguments(arguments);
                        return copy.Matched<IObject>();
                     default:
                        return failedMatch<IObject>(incompatibleClasses(value, "Invokable"));
                  }
               }
               else if (!isFailure)
               {
                  return failedMatch<IObject>(fieldNotFound(image));
               }
               else
               {
                  return failedMatch<IObject>(exception);
               }
            }
            else
            {
               return failedMatch<IObject>(incompatibleClasses(value, "Arguments"));
            }
         }
         else
         {
            return failedMatch<IObject>(popException);
         }
      }

      protected IMatched<IObject> invoke()
      {
         while (!context.Cancelled() && operations.More && running)
         {
            if (operations.Current.If(out var operation))
            {
               trace(operations.Address, () => operation.ToString());
               var currentAddress = operations.Address;
               switch (operation)
               {
                  case Return rtn:
                     return Return.ReturnAction(this, rtn.ReturnTopOfStack);
                  case Yield:
                     return Yield.YieldAction(this);
                  case Invoke invoke:
                     if (Invoke(invoke.FieldName).If(out var returnValue, out var anyException))
                     {
                        stack.Peek().Push(returnValue);
                     }
                     else if (anyException.If(out var exception))
                     {
                        if (GetErrorHandler().If(out var address))
                        {
                           stack.Peek().Push(new Failure(exception.Message));
                           operations.Goto(address);
                        }
                        else
                        {
                           return failedMatch<IObject>(exception);
                        }
                     }

                     if (currentAddress == operations.Address)
                     {
                        operations.Advance(1);
                     }

                     continue;
               }

               if (operation.Execute(this).If(out var result, out var mbResult) && running)
               {
                  stack.Peek().Push(result);
               }
               else if (mbResult.If(out var exception))
               {
                  if (GetErrorHandler().If(out var address))
                  {
                     stack.Peek().Push(new Failure(exception.Message));
                     operations.Goto(address);
                  }
                  else
                  {
                     return failedMatch<IObject>(exception);
                  }
               }

               if (operation.Increment && currentAddress == operations.Address)
               {
                  operations.Advance(1);
               }
            }
            else
            {
               return "Address out of range".FailedMatch<IObject>();
            }
         }

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
         {
            throw "Max stack depth".Throws();
         }
      }

      public void PushFrames(FrameGroup frames)
      {
         foreach (var frame in frames.Reverse())
         {
            PushFrame(frame);
         }
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
            {
               break;
            }
         }

         return new FrameGroup(frames.ToArray());
      }

      public IMaybe<Frame> FunctionFrame()
      {
         foreach (var frame in stack.Where(frame => frame.FrameType == FrameType.Function))
         {
            return frame.Some();
         }

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
            {
               break;
            }
         }

         return new FrameGroup(frames.ToArray());
      }

      public IResult<int> GetErrorHandler()
      {
         while (stack.Count > 0)
         {
            var frame = stack.Pop();
            if (frame.FrameType == FrameType.Try)
            {
               return frame.ErrorHandler.Map(i => i.Success()).DefaultTo(() => "No error handler set".Failure<int>());
            }
         }

         return failure<int>(emptyStack());
      }

      public void Clear() => CurrentFrame.Clear();

      public string StackImage => stack.Select(f => f.ToString()).ToString(", ");

      public IObject X { get; set; } = Unassigned.Value;

      public IObject Y { get; set; } = Unassigned.Value;

      public IObject Z { get; set; } = Unassigned.Value;

      public IMatched<Field> Find(string fieldName, bool getting)
      {
         var depth = 0;
         foreach (var frame in stack)
         {
            if (frame.Fields.Find(fieldName, getting).If(out var field, out var _exception))
            {
               return field.Matched();
            }
            else if (_exception.If(out var exception))
            {
               return failedMatch<Field>(exception);
            }
            else
            {
               if (depth++ > MAX_DEPTH)
               {
                  return failedMatch<Field>(exceededMaxDepth());
               }
            }
         }

         return notMatched<Field>();
      }

      public IResult<Unit> FindByPattern(string pattern, List<Field> list)
      {
         foreach (var frame in stack)
         {
            if (frame.Fields.FindByPattern(pattern, list).IfNot(out var exception))
            {
               return failure<Unit>(exception);
            }
         }

         return Unit.Success();
      }

      public IMatched<Field> Find(Selector selector)
      {
         var labelsOnly = selector.LabelsOnly();
         foreach (var frame in stack)
         {
            if (frame.Fields.ContainsSelector(labelsOnly))
            {
               var match = frame.Fields.Find(selector);
               if (match.If(out var field) && field != null)
               {
                  field.Fields = frame.Fields;
               }

               return match;
            }
         }

         return notMatched<Field>();
      }

      protected IMatched<Field> findExact(Selector selector) => Find(selector.Image, true);

      protected IMatched<Field> findEquivalent(Selector selector)
      {
         var count = selector.SelectorItems.Length;
         var iterator = new BitIterator(count);
         foreach (var booleans in iterator)
         {
            var newSelector = selector.Equivalent(booleans);
            if (findExact(newSelector).If(out var matched, out var anyException))
            {
               return matched.Matched();
            }
            else if (anyException.If(out var exception))
            {
               return failedMatch<Field>(exception);
            }
         }

         return notMatched<Field>();
      }

      protected IMatched<Field> findTypeless(Selector selector) => Find(selector.LabelsOnly().Image, true);

      protected IMatched<Field> findField(Selector selector) => Find(selector.Name, true);

      public IResult<Field> Assign(string fieldName, IObject value, bool getting, bool overriden = false)
      {
         if (Find(fieldName, getting).If(out var field, out var anyException))
         {
            if (field.Mutable)
            {
               if (field.Value is Reference r)
               {
                  r.Field.Value = value;
               }
               else
               {
                  field.Value = value;
               }

               return field.Success();
            }
            else if (field.Value is Unassigned || overriden)
            {
               if (field.Value is Reference r)
               {
                  r.Field.Value = value;
               }
               else
               {
                  field.Value = value;
               }

               return field.Success();
            }
            else
            {
               return failure<Field>(immutableField(fieldName));
            }
         }
         else if (anyException.If(out var exception))
         {
            return failure<Field>(exception);
         }
         else
         {
            return failure<Field>(fieldNotFound(fieldName));
         }
      }

      public IResult<Field> Assign(Selector selector, IObject value, bool overriden = false)
      {
         var ((isFound, field), (isFailure, exception)) = Find(selector);
         if (isFound)
         {
            var fields = field.Fields;
            if (field.Mutable)
            {
               switch (field.Value)
               {
                  case Unassigned:
                     field.Value = value;
                     fields.SetBucket(selector);
                     return field.Success();
                  case TypeConstraint tc2:
                     return failure<Field>(incompatibleClasses(selector, tc2.AsString));
                  default:
                     return failure<Field>(incompatibleClasses(selector, field.Value.ClassName));
               }
            }
            else if (field.Value is Unassigned || overriden)
            {
               field.Value = value;
               fields.SetBucket(selector);
               return field.Success();
            }
            else
            {
               return failure<Field>(immutableField(selector));
            }
         }
         else if (isFailure)
         {
            return failure<Field>(exception);
         }
         else
         {
            return failure<Field>(fieldNotFound(selector));
         }
      }

      public void BeginDebugging()
      {
         debugState = DebugState.Active;
         stack.Clear();
         stack.Push(new Frame());
         operations.Goto(0);
      }

      public void Step()
      {
         if (debugState == DebugState.Starting)
         {
            BeginDebugging();
         }

         while (!context.Cancelled() && operations.More && debugState == DebugState.Active)
         {
            if (operations.Current.If(out var operation))
            {
               var currentAddress = operations.Address;
               switch (operation)
               {
                  case Break:
                     return;
                  default:
                     if (operation.Execute(this).ValueOrOriginal(out var result, out var original) && running &&
                        result.ClassName != "Void")
                     {
                        stack.Peek().Push(result);
                     }
                     else if (original.IfNot(out var _exception) && _exception.If(out _))
                     {
                        return;
                     }

                     break;
               }

               if (operation.Increment && currentAddress == operations.Address)
               {
                  operations.Advance(1);
               }
            }
            else
            {
               return;
            }
         }
      }

      public string PackageFolder { get; set; } = "";

      public IResult<Unit> SetErrorHandler(int address)
      {
         var frame = stack.FirstOrNone(f => f.FrameType == FrameType.Try);
         if (frame.If(out var tf))
         {
            tf.ErrorHandler = address.Some();
            return Unit.Success();
         }
         else
         {
            return "Try frame not found".Failure<Unit>();
         }
      }
   }
}