using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Packages;
using Core.Enumerables;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Kagami.Library.AllExceptions;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;
using Failure = Kagami.Library.Objects.Failure;

namespace Kagami.Library.Runtime;

public class Machine
{
   protected const int MAX_DEPTH = 128;

   public static LateLazy<Machine> Current { get; set; } = new(true);

   public static Fields Fields => Current.Value.CurrentFrame.Fields;

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
         if (operations.Current is (true, var operation))
         {
            trace(operations.Address, () => operation.ToString() ?? "");
            //var currentAddress = operations.Address;
            var _result = operation.Execute(this);
            if (_result is (true, var result) && running && result.ClassName != "Void")
            {
               stack.Peek().Push(result);
            }
            else if (_result.Exception is (true, var exception))
            {
               if (Tracing)
               {
                  context.PrintLine(table.Value.ToString());
               }

               var _errorHandler = GetErrorHandler();
               if (_errorHandler is (true, var address))
               {
                  stack.Peek().Push(new Failure(exception.Message));
                  operations.Goto(address);
               }
               else
               {
                  return exception;
               }
            }

            if (operation.Increment/* && currentAddress == operations.Address*/)
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

   public Optional<IObject> Invoke(IInvokable invokable, Arguments arguments, Fields fields, int increment = 1)
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
         return invoke();
      }
      else
      {
         return badAddress(invokable.Address);
      }
   }

   public Optional<IObject> Invoke(IInvokable invokable, Arguments arguments, int increment = 1)
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

   public Optional<IObject> Invoke(YieldingInvokable invokable)
   {
      var frames = invokable.Frames;
      if (frames.FunctionFrame is (true, var frame))
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

   public Optional<IObject> Invoke(string fieldName)
   {
      var _value = Pop();
      if (_value is (true, var value))
      {
         if (value is Arguments arguments)
         {
            var image = fieldName;
            var _field = Find(fieldName, true);
            if (!_field && !_field.Exception)
            {
               var selector = arguments.Selector(fieldName);
               image = selector.Image;
               _field = Find(selector);
            }

            if (_field is (true, var field))
            {
               value = field.Value;
               switch (value)
               {
                  case IInvokableObject io:
                     return Invoke(io.Invokable, arguments);
                  case IInvokable invokable:
                     return Invoke(invokable, arguments);
                  case PackageFunction pf:
                     return pf.Invoke(arguments).Just();
                  case Pattern pattern:
                     var copy = pattern.Copy();
                     copy.RegisterArguments(arguments);
                     return copy;
                  default:
                     return incompatibleClasses(value, "Invokable");
               }
            }
            else if (!_field.Exception)
            {
               return fieldNotFound(image);
            }
            else
            {
               return _field.Exception;
            }
         }
         else
         {
            return incompatibleClasses(value, "Arguments");
         }
      }
      else
      {
         return _value.Exception;
      }
   }

   protected Optional<IObject> invoke()
   {
      while (!context.Cancelled() && operations.More && running)
      {
         if (operations.Current is (true, var operation))
         {
            trace(operations.Address, () => operation.ToString() ?? "");
            var currentAddress = operations.Address;
            switch (operation)
            {
               case Return rtn:
                  return Return.ReturnAction(this, rtn.ReturnTopOfStack);
               case Yield:
                  return Yield.YieldAction(this).Just();
               case Invoke invoke:
               {
                  var _returnValue = Invoke(invoke.FieldName);
                  if (_returnValue is (true, var returnValue))
                  {
                     stack.Peek().Push(returnValue);
                  }
                  else if (_returnValue.Exception is (true, var exception))
                  {
                     var _address = GetErrorHandler();
                     if (_address is (true, var address))
                     {
                        stack.Peek().Push(new Failure(exception.Message));
                        operations.Goto(address);
                     }
                     else
                     {
                        return _address.Exception;
                     }
                  }

                  /*if (currentAddress == operations.Address)
                  {
                     operations.Advance(1);
                  }*/

                  continue;
               }
            }

            var _result = operation.Execute(this);
            if (_result is (true, var result) && running)
            {
               stack.Peek().Push(result);
            }
            else if (_result.Exception is (true, var exception))
            {
               var _address = GetErrorHandler();
               if (_address is (true, var address))
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
         throw fail("Max stack depth");
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

   public Optional<Field> Find(string fieldName, bool getting)
   {
      var depth = 0;
      foreach (var frame in stack)
      {
         var _field = frame.Fields.Find(fieldName, getting);
         if (_field is (true, var field))
         {
            return field;
         }
         else if (_field.Exception is (true, var exception))
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
         var _found = frame.Fields.FindByPattern(pattern, list);
         if (!_found)
         {
            return _found.Exception;
         }
      }

      return unit;
   }

   public Optional<Field> Find(Selector selector)
   {
      var labelsOnly = selector.LabelsOnly();
      foreach (var frame in stack)
      {
         if (frame.Fields.ContainsSelector(labelsOnly))
         {
            var _field = frame.Fields.Find(selector);
            if (_field is (true, var field))
            {
               field.Fields = frame.Fields;
            }

            return _field;
         }
      }

      return nil;
   }

   protected Optional<Field> findExact(Selector selector) => Find(selector.Image, true);

   protected Optional<Field> findEquivalent(Selector selector)
   {
      var count = selector.SelectorItems.Length;
      var iterator = new BitIterator(count);
      foreach (var booleans in iterator)
      {
         var newSelector = selector.Equivalent(booleans);
         var _field = findExact(newSelector);
         if (_field is (true, var field))
         {
            return field;
         }
         else if (_field.Exception is (true, var exception))
         {
            return exception;
         }
      }

      return nil;
   }

   protected Optional<Field> findTypeless(Selector selector) => Find(selector.LabelsOnly().Image, true);

   protected Optional<Field> findField(Selector selector) => Find(selector.Name, true);

   public Result<Field> Assign(string fieldName, IObject value, bool getting, bool overriden = false)
   {
      var _field = Find(fieldName, getting);
      if (_field is (true, var field))
      {
         if (field.Mutable || field.Value is Unassigned || overriden)
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
      else if (_field.Exception is (true, var exception))
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
      if (_field is (true, var field))
      {
         var fields = field.Fields;
         if (field.Mutable)
         {
            switch (field.Value)
            {
               case Unassigned:
                  field.Value = value;
                  fields.SetBucket(selector);

                  return field;
               case TypeConstraint tc2:
                  return incompatibleClasses(selector, tc2.AsString);
               default:
                  return incompatibleClasses(selector, field.Value.ClassName);
            }
         }
         else if (field.Value is Unassigned || overriden)
         {
            field.Value = value;
            fields.SetBucket(selector);

            return field;
         }
         else
         {
            return immutableField(selector);
         }
      }
      else if (_field.Exception is (true, var exception))
      {
         return exception;
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
         if (operations.Current is (true, var operation))
         {
            var currentAddress = operations.Address;
            switch (operation)
            {
               case Break:
                  return;
               default:
               {
                  var _result = operation.Execute(this);
                  if (_result is (true, var result) && running && result.ClassName != "Void")
                  {
                     stack.Peek().Push(result);
                  }
                  else if (_result)
                  {
                     return;
                  }

                  break;
               }
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
      if (_frame is (true, var frame))
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