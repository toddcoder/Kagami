﻿using System;
using System.Collections.Generic;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Parsers;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Operations
{
   public class OperationsBuilder
   {
      List<Operation> operations;
      Hash<string, int> labels;
      Hash<int, string> addresses;
      Hash<int, IInvokable> invokables;
      Hash<int, Block> blocks;
      Hash<LabelType, Stack<string>> labelStack;
      Set<string> registeredBlocks;

      public OperationsBuilder()
      {
         operations = new List<Operation>();
         labels = new AutoHash<string, int>(k => -1);
         addresses = new AutoHash<int, string>(k => "");
         invokables = new Hash<int, IInvokable>();
         blocks = new Hash<int, Block>();
         labelStack = new AutoHash<LabelType, Stack<string>>(type => new Stack<string>());
         registeredBlocks = new Set<string>();
      }

      public IResult<int> RegisterInvokable(IInvokable invokable, Block block, bool overriding)
      {
         if (invokables.ContainsKey(invokable.Index) && !overriding)
            return $"Invokable {invokable.Image} already registered".Failure<int>();

/*         if (registeredBlocks.Contains(block.ToString()))
            return (-1).Success();*/

         var index = invokables.Count;
         invokable.Index = index;
         invokables[index] = invokable;
         blocks[index] = block;
         registeredBlocks.Add(block.ToString());

         return index.Success();
      }

      public IResult<int> RegisterInvokable(IInvokable invokable, Expression expression, bool overriding)
      {
         return RegisterInvokable(invokable, new Block(new ExpressionStatement(expression, true)), overriding);
      }

      void add(Operation operation) => operations.Add(operation);

      public void AddRaw(Operation operation) => operations.Add(operation);

      string add(AddressedOperation operation, string label)
      {
         addresses[operations.Count] = label;
         operations.Add(operation);

         return label;
      }

      public void Merge(OperationsBuilder otherBuilder)
      {
         operations.AddRange(otherBuilder.operations);
         foreach (var item in otherBuilder.addresses)
            addresses[item.Key] = item.Value;
         foreach (var item in labels)
            labels[item.Key] = item.Value;
      }

      public void Label(string label) => labels[label] = operations.Count;

      public string PushLabel(LabelType type, string name)
      {
         var label = newLabel(name);
         if (labelStack.If(type, out var stack))
            stack.Push(label);
         else
         {
            stack = new Stack<string>();
            stack.Push(label);
            labelStack[type] = stack;
         }

         return label;
      }

      public string PeekLabel(LabelType type) => labelStack.FlatMap(type, stack => stack.Peek(), () => "");

      public string PopLabel(LabelType type) => labelStack[type].Pop();

      public void Label(LabelType type) => Label(PeekLabel(type));

      public void PushInt(int value) => add(new PushInt(value));

      public void PushFloat(double value) => add(new PushFloat(value));

      public void PushBoolean(bool value) => add(new PushBoolean(value));

      public void PushString(string value) => add(new PushString(value));

      public void PushChar(char value) => add(new PushChar(value));

      public void PushByte(byte value) => add(new PushByte(value));

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

      public string GoTo(string label) => add(new GoTo(), label);

      public string GoTo(LabelType type) => add(new GoTo(), PeekLabel(type));

      public string GoToIfTrue(string label) => add(new GoToIfTrue(), label);

      public string GoToIfFalse(string label) => add(new GoToIfFalse(), label);

      public string GoToIfSome(string label) => add(new GoToIfSome(), label);

      public string GoToIfNil(string label) => add(new GoToIfNil(), label);

      public void Compare() => add(new Compare());

      public void IsZero() => add(new IsZero());

      public void IsPositive() => add(new IsPositive());

      public void IsNegative() => add(new IsNegative());

      public void Stop() => add(new Stop());

      public void Invoke(string functionName, params Expression[] arguments)
      {
         foreach (var argument in arguments)
            argument.Generate(this);
         Invoke(functionName, arguments.Length);
      }

      public void Invoke(string functionName, int count)
      {
         ToArguments(count);
         add(new Invoke(functionName));
      }

      public void PostfixInvoke() => add(new PostfixInvoke());

      public void Return(bool returnTopOfStack) => add(new Return(returnTopOfStack));

      public void GetField(string name) => add(new GetField(name));

      public void NewField(string name, bool mutable, bool visible) => add(new NewField(name, mutable, visible));

      public void AssignField(string name, bool overriding) => add(new AssignField(name, overriding));

      public void SendMessage(string message, params Expression[] arguments)
      {
         foreach (var argument in arguments)
            argument.Generate(this);
         SendMessage(message, arguments.Length);
      }

      public void SendMessage(string message, int count)
      {
         ToArguments(count);
         add(new SendMessage(message));
      }

      public void Negate() => add(new Negate());

      public void Image() => add(new Image());

      public void Peek(int index) => add(new Peek(index));

      public void EndOfLine() => add(new EndOfLine());

      public void String() => add(new AsString());

      public void Equal() => add(new Equal());

      public void Not() => add(new Not());

      public void Dup() => add(new Dup());

      public void Dup2() => add(new Dup2());

      public void Swap() => add(new Swap());

      public void Rotate(int count) => add(new Rotate(count));

      public void Roll(int count) => add(new Roll(count));

      public void And() => add(new And());

      public void Or() => add(new Or());

      public void NoOp() => add(new NoOp());

      public void Advance(int increment) => add(new Advance(increment));

      public void NewNameValue() => add(new NewNameValue());

      public void NewInternalList() => add(new NewInternalList());

      public void NewTuple() => add(new NewTuple());

      public void NewList() => add(new NewList());

      //public void OneTuple() => add(new OneTuple());

      //public void ToTuple() => add(new ToTuple());

      /*public void ToTuple(int count)
      {
         PushInt(count);
         ToTuple();
      }*/

      public void PushFrame() => add(new PushFrame());

      public void PushFrameWithValue() => add(new PushFrameWithValue());

      public void PopFrame() => add(new PopFrame());

      public void PopFrameWithValue() => add(new PopFrameWithValue());

      public void PushExitFrame(string label) => add(new PushExitFrame(), label);

      public void PopExitFrame() => add(new PopExitFrame());

      public void PushSkipFrame(string label) => add(new PushSkipFrame(), label);

      public void PopSkipFrame() => add(new PopSkipFrame());

      public void NewArray() => add(new NewArray());

      public void GetIterator(bool lazy) => add(new GetIterator(lazy));

      //public void EmptyTuple() => add(new EmptyTuple());

      public void NewRange(bool inclusive) => add(new NewRange(inclusive));

      public void IsClass(string className, bool pop) => add(new IsClass(className, pop));

      public void Match(bool mutable, bool strict) => add(new Match(mutable, strict));

      public void Match() => add(new Match());

      public void Drop() => add(new Drop());

      public void Some() => add(new Some());

      public void PushNil() => add(new PushNil());

      public void Yield() => add(new Yield());

      public void NewObject(string className, Parameters parameters) => add(new NewObject(className, parameters));

      public void FieldsFromObject() => add(new FieldsFromObject());

      public void NewDataType(string className) => add(new NewDataType(className));

      public void NewDataComparisand() => add(new NewDataComparisand());

      public void FieldExists(string fieldName) => add(new FieldExists(fieldName));

      public void NewRational() => add(new NewRational());

      public void NewKeyValue() => add(new NewKeyValue());

      public void NewValue(string className, Func<Arguments, IObject> initiallizer) => add(new NewValue(className, initiallizer));

      public void AssignMetaObject(string className, string metaClassName) => add(new AssignMetaObject(className, metaClassName));

      public void Super() => add(new Super());

      public void SetX() => add(new SetX());

      public void GetX() => add(new GetX());

      public void RespondsTo(string message) => add(new RespondsTo(message));

      public void ToArguments(int count)
      {
         PushInt(count);
         add(new ToArguments());
      }

      public void Pipeline() => add(new Pipeline());

      public void BAnd() => add(new BAnd());

      public void BOr() => add(new BOr());

      public void BXor() => add(new BXor());

      public void BShiftLeft() => add(new BShiftLeft());

      public void BShiftRight() => add(new BShiftRight());

      public void BNot() => add(new BNot());

      public void NewOpenRange() => add(new NewOpenRange());

      public IResult<Operations> ToOperations(ParseState state)
      {
         operations.Add(new Stop());
         for (var i = 0; i < invokables.Count; i++)
         {
            var invokable = invokables[i];
            var block = blocks[i];

            invokable.Address = operations.Count;
            block.Generate(this);
            var lastOperation = operations[operations.Count - 1];
            if (!(lastOperation is Return) && !(lastOperation is NoOp))
               operations.Add(new Return(false));
         }

         foreach (var symbol in state.PostGenerationSymbols)
         {
            var invokable = ((IInvokableObject)symbol).Invokable;
            invokable.Address = operations.Count;
            symbol.Generate(this);
            var lastOperation = operations[operations.Count - 1];
            if (!(lastOperation is Return) && !(lastOperation is NoOp))
               operations.Add(new Return(false));
         }

         foreach (var item in addresses)
         {
            var address = labels[item.Value];
            if (operations[item.Key] is AddressedOperation op)
               if (address > -1)
                  op.Address = address;
               else
                  return $"Label {item.Value} couldn't be found".Failure<Operations>();
            else
               return $"Addressed operation required; {operations[item.Key]} found".Failure<Operations>();
         }

         return new Operations(operations.ToArray()).Success();
      }
   }
}