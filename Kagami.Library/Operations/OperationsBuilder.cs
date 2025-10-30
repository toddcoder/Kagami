using Kagami.Library.Invokables;
using Kagami.Library.Nodes;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Packages;
using Kagami.Library.Parsers;
using Core.Collections;
using Core.Monads;
using static Kagami.Library.Nodes.NodeFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class OperationsBuilder
{
   protected List<Operation> operations = [];
   protected Memo<string, int> labels = new Memo<string, int>.Value(-1);
   protected Memo<int, string> addresses = new Memo<int, string>.Function(_ => "");
   protected Hash<int, IInvokable> invokables = [];
   protected Hash<int, Block> blocks = [];
   // ReSharper disable once CollectionNeverUpdated.Global
   protected Memo<LabelType, Stack<string>> labelStack = new Memo<LabelType, Stack<string>>.Function(_ => new Stack<string>());
   protected Set<string> registeredBlocks = [];
   protected Stack<MacroParameters> macroParameters = new();
   protected Stack<string> returnLabels = new();

   public Result<int> RegisterInvokable(IInvokable invokable, Block block, bool overriding)
   {
      if (invokables.ContainsKey(invokable.Index) && !overriding)
      {
         return invokable.Index;
      }
      else
      {
         var index = invokables.Count;
         invokable.Index = index;
         invokables[index] = invokable;
         blocks[index] = block;
         registeredBlocks.Add(block.ToString());

         return index;
      }
   }

   public Result<int> RegisterInvokable(IInvokable invokable, Expression expression, bool overriding)
   {
      return RegisterInvokable(invokable, new Block(new ExpressionStatement(expression, true)), overriding);
   }

   protected void add(Operation operation) => operations.Add(operation);

   public void AddRaw(Operation operation) => operations.Add(operation);

   protected void add(AddressedOperation operation, string label)
   {
      addresses[operations.Count] = label;
      operations.Add(operation);
   }

   public void Merge(OperationsBuilder otherBuilder)
   {
      operations.AddRange(otherBuilder.operations);
      foreach (var (key, value) in otherBuilder.addresses)
      {
         addresses[key] = value;
      }

      foreach (var (key, value) in labels)
      {
         labels[key] = value;
      }
   }

   public void Label(string label) => labels[label] = operations.Count;

   public void PushLabel(LabelType type, string name)
   {
      var label = newLabel(name);
      labelStack[type].Push(label);
   }

   public string PeekLabel(LabelType type) => labelStack[type].Peek();

   public void PopLabel(LabelType type) => labelStack[type].Pop();

   public void Label(LabelType type) => Label(PeekLabel(type));

   public void CallSysFunction0(Func<Sys, Result<IObject>> func, string image) => add(new CallSysFunction0(func, image));

   public void CallSysFunction1(Func<Sys, IObject, Result<IObject>> func, string image) => add(new CallSysFunction1(func, image));

   public void CallSysFunction2(Func<Sys, IObject, IObject, Result<IObject>> func, string image) =>
      add(new CallSysFunction2(func, image));

   public void PushInt(int value) => add(new PushInt(value));

   public void PushFloat(double value) => add(new PushFloat(value));

   public void PushBoolean(bool value) => add(new PushBoolean(value));

   public void PushString(string value) => add(new PushString(value));

   public void PushChar(char value) => add(new PushChar(value));

   public void PushByte(byte value) => add(new PushByte(value));

   public void PushDecimal(decimal value) => add(new PushDecimal(value));

   public void PushObject(IObject obj) => add(new PushObject(obj));

   public void Print() => add(new Print());

   public void PrintLine() => add(new PrintLine());

   public void Put() => add(new Put());

   public void ReadLine() => add(new ReadLine());

   public void Add() => add(new Add());

   public void Subtract() => add(new Subtract());

   public void Multiply() => add(new Multiply());

   public void FloatDivide() => add(new FloatDivide());

   public void IntDivide() => add(new IntDivide());

   public void Remainder() => add(new Remainder());

   public void Raise() => add(new Raise());

   public void GoTo(string label) => add(new GoTo(), label);

   public void GoTo(LabelType type) => add(new GoTo(), PeekLabel(type));

   public void GoToIfTrue(string label) => add(new GoToIfTrue(), label);

   public void GoToIfFalse(string label) => add(new GoToIfFalse(), label);

   public void GoToIfSome(string label) => add(new GoToIfSome(), label);

   public void GoToIfNil(string label) => add(new GoToIfNil(), label);

   public void GoToIfSuccess(string label) => add(new GoToIfSuccess(), label);

   public void GoToIfFailure(string label) => add(new GoToIfFailure(), label);

   public void Compare() => add(new Compare());

   public void IsZero() => add(new IsZero());

   public void IsPositive() => add(new IsPositive());

   public void IsNegative() => add(new IsNegative());

   public void Stop() => add(new Stop());

   public void Invoke(string functionName, params Expression[] arguments)
   {
      foreach (var argument in arguments)
      {
         argument.Generate(this);
      }

      Invoke(functionName, arguments.Length);
   }

   public void Invoke(string functionName, int count)
   {
      ToArguments(count);
      add(new Invoke(functionName));
   }

   public void InvokeWithoutArguments(string functionName) => add(new Invoke(functionName));

   public void PostfixInvoke() => add(new PostfixInvoke());

   public void Return(bool returnTopOfStack) => add(new Return(returnTopOfStack));

   public void ReturnType(bool returnTopOfStack, TypeConstraint typeConstraint)
   {
      add(new ReturnType(returnTopOfStack, typeConstraint));
   }

   public void GetField(string name) => add(new GetField(name));

   public void NewField(string name, bool mutable, bool visible, Maybe<TypeConstraint> _typeConstraint)
   {
      add(new NewField(name, mutable, visible, _typeConstraint));
   }

   public void NewField(string name, bool mutable, bool visible)
   {
      add(new NewField(name, mutable, visible, nil));
   }

   public void NewFieldTolerant(string name, bool mutable, bool visible, Maybe<TypeConstraint> _typeConstraint) =>
      add(new NewFieldTolerant(name, mutable, visible, _typeConstraint));

   public void NewFieldTolerant(string name, bool mutable, bool visible) => add(new NewFieldTolerant(name, mutable, visible, nil));

   public void NewSelector(Selector selector, bool mutable, bool visible) => add(new NewSelector(selector, mutable, visible));

   public void AssignField(string name, bool overriding) => add(new AssignField(name, overriding));

   public void StoreField(string name, bool mutable, bool visible, Maybe<TypeConstraint> _typeConstraint) =>
      add(new StoreField(name, mutable, visible, _typeConstraint));

   public void AssignSelector(Selector selector, bool overriding) => add(new AssignSelector(selector, overriding));

   public void AssignFieldReference(string sourceFieldName, string targetFieldName)
   {
      add(new AssignFieldReference(sourceFieldName, targetFieldName));
   }

   public void SendMessage(Selector selector, params Expression[] arguments)
   {
      foreach (var argument in arguments)
      {
         argument.Generate(this);
      }

      SendMessage(selector, arguments.Length);
   }

   public void SendMessage(Selector selector, int count)
   {
      ToArguments(count);
      add(new SendMessage(selector));
   }

   public void NewMessage(Selector selector, int count)
   {
      ToArguments(count);
      add(new NewMessage(selector));
   }

   public void Negate() => add(new Negate());

   public void Abs() => add(new Abs());

   public void Image() => add(new Image());

   public void EndOfLine() => add(new EndOfLine());

   public void String() => add(new AsString());

   public void Equal() => add(new Equal());

   public void LessThan() => add(new LessThan());

   public void LessThanEqual() => add(new LessThanEqual());

   public void GreaterThan() => add(new GreaterThan());

   public void GreaterThanEqual() => add(new GreaterThanEqual());

   public void Not() => add(new Not());

   public void Dup() => add(new Dup());

   public void Dup2() => add(new Dup2());

   public void Swap() => add(new Swap());

   public void SwapAt(int index) => add(new SwapAt(index));

   public void Pick(int index) => add(new Pick(index));

   public void Copy(int index) => add(new Copy(index));

   public void Rotate(int count) => add(new Rotate(count));

   public void Roll(int count) => add(new Roll(count));

   public void And() => add(new And());

   public void Or() => add(new Or());

   public void XOr() => add(new XOr());

   public void NoOp() => add(new NoOp());

   public void Advance(int increment) => add(new Advance(increment));

   public void NewNameValue() => add(new NewNameValue());

   public void NewSequence() => add(new NewSequence());

   public void NewTuple() => add(new NewTuple());

   public void NewMonoTuple() => add(new NewMonoTuple());

   public void NewList() => add(new NewList());

   public void NewLambda(IInvokable invokable, bool captures) => add(new NewLambda(invokable, captures));

   public void NewSkipTake() => add(new NewSkipTake());

   public void PushFrame() => add(new PushFrame());

   public void PushFunctionFrame() => add(new PushFunctionFrame());

   public void PushFrameWithValue() => add(new PushFrameWithValue());

   public void PushFrameWithArguments() => add(new PushFrameWithArguments());

   public void PopFrame() => add(new PopFrame());

   public void PopFrameWithValue() => add(new PopFrameWithValue());

   public void PushExitFrame(string label) => add(new PushExitFrame(), label);

   public void PopExitFrame() => add(new PopExitFrame());

   public void PushSkipFrame(string label) => add(new PushSkipFrame(), label);

   public void PopSkipFrame() => add(new PopSkipFrame());

   public void PopTryFrame() => add(new PopTryFrame());

   public void NewArray() => add(new NewArray());

   public void NewTypedArray(TypeConstraint typeConstraint) => add(new NewTypedArray(typeConstraint));

   public void NewDictionaryOrSet() => add(new NewDictionaryOrSet());

   public void NewCycle() => add(new NewCycle());

   public void GetIterator(bool lazy) => add(new GetIterator(lazy));

   public void NewRange(bool inclusive) => add(new NewRange(inclusive));

   public void IsClass(string className, bool pop) => add(new IsClass(className, pop));

   public void IsUserClass() => add(new IsUserClass());

   public void Match() => add(new Match());

   public void Drop() => add(new Drop());

   public void Some() => add(new Some());

   public void Success() => add(new Success());

   public void PushNil() => add(new PushNil());

   public void Failure() => add(new Failure());

   public void Yield() => add(new Yield());

   public void NewObject(string className, Parameters parameters) => add(new NewObject(className, parameters));

   public void FieldsFromObject() => add(new FieldsFromObject());

   public void NewDataType(string className) => add(new NewDataType(className));

   public void NewDataComparisand() => add(new NewDataComparisand());

   public void FieldExists(string fieldName) => add(new FieldExists(fieldName));

   public void NewRational() => add(new NewRational());

   public void NewKeyValue() => add(new NewKeyValue());

   public void NewValue(string className, Func<Arguments, IObject> initializer) => add(new NewValue(className, initializer));

   public void AssignMetaObject(string className, string metaClassName) => add(new AssignMetaObject(className, metaClassName));

   public void Super() => add(new Super());

   public void SetX() => add(new SetX());

   public void GetX() => add(new GetX());

   public void RespondsTo(string message) => add(new RespondsTo(message));

   public void ToArguments(int count)
   {
      PushInt(count);
      add(new NewArguments());
   }

   public void Pipeline() => add(new Pipeline());

   public void BackPipeline() => add(new BackPipeline());

   public void NewOpenRange() => add(new NewOpenRange());

   public void SetFields(Parameters parameters) => add(new SetFields(parameters));

   public void Break() => add(new Reset());

   public void OpenPackage(string packageName) => add(new OpenPackage(packageName));

   public void OpenEnum(string enumName) => add(new OpenEnum(enumName));

   public void ImportPackage(string packageName) => add(new ImportPackage(packageName));

   public void TryBegin(string label) => add(new TryBegin(), label);

   public void TryEnd() => add(new TryEnd());

   public void SkipTake() => add(new SkipTake());

   public void SetErrorHandler(string label) => add(new SetErrorHandler(), label);

   public void Throw() => add(new Throw());

   public void DivRem() => add(new DivRem());

   public void Convert() => add(new Convert());

   public void ArgumentLabel(string label) => add(new ArgumentLabel(label));

   public void AssignFieldWithType(string fieldName, string className) => add(new AssignFieldWithType(fieldName, className));

   public void LastValue() => add(new LastValue());

   public void DefineNewField(bool mutable, string fieldName, string className) => add(new DefineNewField(mutable, fieldName, className));

   public void Increment() => add(new Increment());

   public void Decrement() => add(new Decrement());

   public void NewCons() => add(new NewCons());

   public void Defer() => add(new Defer());

   public void Column() => add(new Column());

   public Result<Operations> ToOperations(ParseState state)
   {
      operations.Add(new Stop());
      for (var i = 0; i < invokables.Count; i++)
      {
         var invokable = invokables[i];
         var block = blocks[i];

         invokable.Address = operations.Count;
         block.Generate(this);
         var lastOperation = operations[^1];
         if (!(lastOperation is Return))
         {
            operations.Add(new Return(false));
         }
      }

      foreach (var symbol in state.PostGenerationSymbols)
      {
         var invokable = ((IInvokableObject)symbol).Invokable;
         invokable.Address = operations.Count;
         symbol.Generate(this);
         var lastOperation = operations[^1];
         if (lastOperation is not Library.Operations.Return)
         {
            operations.Add(new Return(false));
         }
      }

      foreach (var (key, value) in addresses)
      {
         var address = labels[value];
         if (operations[key] is AddressedOperation op)
         {
            if (address > -1)
            {
               op.Address = address;
            }
            else
            {
               return $"Label {value} couldn't be found".Failure<Operations>();
            }
         }
         else
         {
            return $"Addressed operation required; {operations[key]} found".Failure<Operations>();
         }
      }

      return new Operations([.. operations]);
   }

   public void BeginMacro(Parameters parameters, Expression[] arguments, string returnLabel = "")
   {
      var macroParameter = new MacroParameters();
      macroParameter.Assign(parameters, arguments);
      macroParameters.Push(macroParameter);
      returnLabels.Push(returnLabel);
   }

   public void EndMacro()
   {
      macroParameters.Pop();
      returnLabels.Pop();
   }

   public void Field(Symbol symbol)
   {
      if (macroParameters.Any(macroParameter => macroParameter.Replace(symbol, this)))
      {
         return;
      }

      if (symbol is FieldSymbol fieldSymbol)
      {
         GetField(fieldSymbol.FieldName);
      }
   }

   public void ReturnNothing()
   {
      if (returnLabels.Count == 0)
      {
         Return(false);
      }
      else
      {
         GoTo(returnLabels.Peek());
      }
   }

   public void Return(Expression expression, Statement statement)
   {
      expression.Generate(this);

      if (returnLabels.Count == 0)
      {
         Return(true);
      }
      else
      {
         GoTo(returnLabels.Peek());
      }
   }

   public void Return(Expression expression, Statement statement, Maybe<TypeConstraint> _typeConstraint)
   {
      expression.Generate(this);

      if (returnLabels.Count == 0)
      {
         if (_typeConstraint is (true, var typeConstraint))
         {
            ReturnType(true, typeConstraint);
         }
         else
         {
            Return(true);
         }
      }
      else
      {
         GoTo(returnLabels.Peek());
      }
   }

   public void RequireFunction(Selector selector) => add(new RequireFunction(selector));

   public void PreIncrement(bool increment) => add(increment ? new PreIncrement() : new PreDecrement());

   public void PostIncrement(bool increment) => add(increment ? new PostIncrement() : new PostDecrement());

   public void Concatenate() => add(new Concatenate());

   public void SetRegister(int index) => add(new SetRegister(index));

   public void GetRegister(int index) => add(new GetRegister(index));

   public void ClearRegister(int index) => add(new ClearRegister(index));

   public void NewBinding(string name) => add(new NewBinding(name));

   public void IsOptional() => add(new IsOptional());

   public void IsResult() => add(new IsResult());

   public void IsMonad(MonadType type) => add(new IsMonad(type));

   public void UnwrapMonad() => add(new UnwrapMonad());

   public void FieldNameFromId() => add(new FieldNameFromId());

   public void GetId() => add(new GetId());

   public void PopNewField(bool mutable, bool visible = true) => add(new PopNewField(mutable, visible));

   public void PopGetField() => add(new PopGetField());

   /*public void AssignLastSome() => add(new AssignLastSome());

   public void AssignLastSuccess() => add(new AssignLastSuccess());*/

   public void LambdaFromSelector() => add(new LambdaFromSelector());

   public void LambdaCapture() => add(new LambdaCapture());

   public void RunTimeArguments() => add(new RunTimeArguments());

   public void ClassName() => add(new ClassName());

   public void UniqueString() => add(new UniqueString());

   public void Cons() => add(new Cons());

   public void NewFailure() => add(new NewFailure());

   public void IsTrue() => add(new IsTrue());

   public void NewDelegate(string className, string delegateClassName) => add(new NewDelegate(className, delegateClassName));

   public void NewSpecialComparisand(SpecialComparisandDirection direction) => add(new NewSpecialComparisand(direction));

   public void NewMutString(string text) => add(new NewMutString(text));

   public void NewJunction(string junctionType) => add(new NewJunction(junctionType));

   public void HasAnyJunctions() => add(new HasAnyJunctions());

   public void JunctionInvoke(string functionName) => add(new JunctionInvoke(functionName));

   public void NewSlip() => add(new NewSlip());

   public void NameOf(string name, bool isClass) => add(new NameOf(name, isClass));

   public void Defined(string name, bool isClass) => add(new Defined(name, isClass));

   public void RegisterAutoConversion(string fromClass, string toClass) => add(new RegisterAutoConversion(fromClass, toClass));

   public override string ToString() => "operations";
}