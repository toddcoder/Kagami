using System;
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

      public Result<Unit> Execute()
      {
         stack.Clear();
         globalFrame = new GlobalFrame();
         stack.Push(globalFrame);
         operations.Goto(0);
         running = true;

         while (!context.Cancelled() && operations.More && running)
         {
            if (operations.Current.Map(out var operation))
            {
               trace(operations.Address, () => operation.ToString());
               var currentAddress = operations.Address;
               if (operation.Execute(this).Map(out var result, out var _exception) && running && result.ClassName != "Void")
               {
                  stack.Peek().Push(result);
               }
               else if (_exception.UnMap(out var exception))
               {
                  if (Tracing)
                  {
                     context.PrintLine(table.Value.ToString());
                  }

                  if (GetErrorHandler().Map(out var address))
                  {
                     stack.Peek().Push(new Failure(exception.Message));
                     operations.Goto(address);
                  }
                  else
                  {
                     return exception;
                  }
               }

               if (operation.Increment && currentAddress == operations.Address)
               {
                  operations.Advance(1);
               }
            }
            else
            {
               return addressOutOfRange();
            }
         }

         if (Tracing)
         {
            context.PrintLine(table.Value.ToString());
         }

         return unit;
      }

      public Responding<IObject> Invoke(IInvokable invokable, Arguments arguments, Fields fields, int increment = 1)
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
            return badAddress(invokable.Address);
         }
      }

      public Responding<IObject> Invoke(IInvokable invokable, Arguments arguments, int increment = 1)
      {
         var returnAddress = Address + increment;
         var frame = new Frame(returnAddress, arguments);

         if (invokable is YieldingInvokable yfi)
         {
            yfi.Arguments = arguments;
         }

         PushFrame(frame);
         frame.SetFields(invokable.Parameters);

         return GoTo(invokable.Address) ? invoke() : badAddress(invokable.Address);
      }

      public Responding<IObject> Invoke(YieldingInvokable invokable)
      {
         var frames = invokable.Frames;
         if (frames.FunctionFrame.Map(out var frame))
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

      public Responding<IObject> Invoke(string fieldName)
      {
         if (Pop().Map(out var value, out var popException))
         {
            if (value is Arguments arguments)
            {
               var image = fieldName;
               var _field = Find(fieldName, true);
               if (!_field && !_field.AnyException)
               {
                  var selector = arguments.Selector(fieldName);
                  image = selector.Image;
                  _field = Find(selector);
               }

               if (_field)
               {
                  value = _field.Value.Value;
                  switch (value)
                  {
                     case IInvokableObject io:
                        return Invoke(io.Invokable, arguments);
                     case IInvokable invokable:
                        return Invoke(invokable, arguments);
                     case PackageFunction pf:
                        return pf.Invoke(arguments).Response();
                     case Pattern pattern:
                        var copy = pattern.Copy();
                        copy.RegisterArguments(arguments);
                        return copy.Response<IObject>();
                     default:
                        return incompatibleClasses(value, "Invokable");
                  }
               }
               else if (!_field.AnyException)
               {
                  return fieldNotFound(image);
               }
               else
               {
                  return _field.AnyException.Value;
               }
            }
            else
            {
               return incompatibleClasses(value, "Arguments");
            }
         }
         else
         {
            return popException;
         }
      }

      protected Responding<IObject> invoke()
      {
         while (!context.Cancelled() && operations.More && running)
         {
            if (operations.Current.Map(out var operation))
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
                     if (Invoke(invoke.FieldName).Map(out var returnValue, out var _exception))
                     {
                        stack.Peek().Push(returnValue);
                     }
                     else if (_exception.Map(out var exception))
                     {
                        if (GetErrorHandler().Map(out var address))
                        {
                           stack.Peek().Push(new Failure(exception.Message));
                           operations.Goto(address);
                        }
                        else
                        {
                           return exception;
                        }
                     }

                     if (currentAddress == operations.Address)
                     {
                        operations.Advance(1);
                     }

                     continue;
               }

               if (operation.Execute(this).Map(out var result, out var _exception1) && running)
               {
                  stack.Peek().Push(result);
               }
               else if (_exception1.Map(out var exception))
               {
                  if (GetErrorHandler().Map(out var address))
                  {
                     stack.Peek().Push(new Failure(exception.Message));
                     operations.Goto(address);
                  }
                  else
                  {
                     return exception;
                  }
               }

               if (operation.Increment && currentAddress == operations.Address)
               {
                  operations.Advance(1);
               }
            }
            else
            {
               return fail("Address out of range");
            }
         }

         return fail("No return");
      }

      public bool Running
      {
         get => running;
         set => running = value;
      }

      public void Push(IObject value) => stack.Peek().Push(value);

      public Maybe<IObject> Peek() => stack.Peek().Peek();

      public Result<IObject> Pop() => stack.Peek().Pop();

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

      public Result<Frame> PopFrame() => tryTo(() => stack.Pop());

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

      public Maybe<Frame> FunctionFrame()
      {
         foreach (var frame in stack.Where(frame => frame.FrameType == FrameType.Function))
         {
            return frame;
         }

         return nil;
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

      public Result<int> GetErrorHandler()
      {
         while (stack.Count > 0)
         {
            var frame = stack.Pop();
            if (frame.FrameType == FrameType.Try)
            {
               return frame.ErrorHandler.Result("No error handler set");
            }
         }

         return emptyStack();
      }

      public void Clear() => CurrentFrame.Clear();

      public string StackImage => stack.Select(f => f.ToString()).ToString(", ");

      public IObject X { get; set; } = Unassigned.Value;

      public IObject Y { get; set; } = Unassigned.Value;

      public IObject Z { get; set; } = Unassigned.Value;

      public Responding<Field> Find(string fieldName, bool getting)
      {
         var depth = 0;
         foreach (var frame in stack)
         {
            if (frame.Fields.Find(fieldName, getting).Map(out var field, out var _exception))
            {
               return field;
            }
            else if (_exception.Map(out var exception))
            {
               return exception;
            }
            else
            {
               if (depth++ > MAX_DEPTH)
               {
                  return exceededMaxDepth();
               }
            }
         }

         return nil;
      }

      public Result<Unit> FindByPattern(string pattern, List<Field> list)
      {
         foreach (var frame in stack)
         {
            if (frame.Fields.FindByPattern(pattern, list).UnMap(out var exception))
            {
               return exception;
            }
         }

         return unit;
      }

      public Responding<Field> Find(Selector selector)
      {
         var labelsOnly = selector.LabelsOnly();
         foreach (var frame in stack)
         {
            if (frame.Fields.ContainsSelector(labelsOnly))
            {
               var match = frame.Fields.Find(selector);
               if (match.Map(out var field) && field != null)
               {
                  field.Fields = frame.Fields;
               }

               return match;
            }
         }

         return nil;
      }

      protected Responding<Field> findExact(Selector selector) => Find(selector.Image, true);

      protected Responding<Field> findEquivalent(Selector selector)
      {
         var count = selector.SelectorItems.Length;
         var iterator = new BitIterator(count);
         foreach (var booleans in iterator)
         {
            var newSelector = selector.Equivalent(booleans);
            if (findExact(newSelector).Map(out var matched, out var _exception))
            {
               return matched;
            }
            else if (_exception.Map(out var exception))
            {
               return exception;
            }
         }

         return nil;
      }

      protected Responding<Field> findTypeless(Selector selector) => Find(selector.LabelsOnly().Image, true);

      protected Responding<Field> findField(Selector selector) => Find(selector.Name, true);

      public Result<Field> Assign(string fieldName, IObject value, bool getting, bool overriden = false)
      {
         if (Find(fieldName, getting).Map(out var field, out var _exception))
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

               return field;
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

               return field;
            }
            else
            {
               return immutableField(fieldName);
            }
         }
         else if (_exception.Map(out var exception))
         {
            return exception;
         }
         else
         {
            return fieldNotFound(fieldName);
         }
      }

      public Result<Field> Assign(Selector selector, IObject value, bool overriden = false)
      {
         var _field = Find(selector);
         if (_field)
         {
            var fields = _field.Value.Fields;
            if (_field.Value.Mutable)
            {
               switch (_field.Value.Value)
               {
                  case Unassigned:
                     _field.Value.Value = value;
                     fields.SetBucket(selector);
                     return _field.Value;
                  case TypeConstraint tc2:
                     return incompatibleClasses(selector, tc2.AsString);
                  default:
                     return incompatibleClasses(selector, _field.Value.Value.ClassName);
               }
            }
            else if (_field.Value.Value is Unassigned || overriden)
            {
               _field.Value.Value = value;
               fields.SetBucket(selector);
               return _field.Value;
            }
            else
            {
               return immutableField(selector);
            }
         }
         else if (_field.AnyException)
         {
            return _field.AnyException.Value;
         }
         else
         {
            return fieldNotFound(selector);
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
            if (operations.Current.Map(out var operation))
            {
               var currentAddress = operations.Address;
               switch (operation)
               {
                  case Break:
                     return;
                  default:
                     if (operation.Execute(this).Map(out var result, out var _exception) && running && result.ClassName != "Void")
                     {
                        stack.Peek().Push(result);
                     }
                     else if (_exception)
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

      public Result<Unit> SetErrorHandler(int address)
      {
         var _frame = stack.FirstOrNone(f => f.FrameType == FrameType.Try);
         if (_frame.Map(out var frame))
         {
            frame.ErrorHandler = address;
            return unit;
         }
         else
         {
            return fail("Try frame not found");
         }
      }
   }
}