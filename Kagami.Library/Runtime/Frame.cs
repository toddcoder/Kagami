﻿using System;
using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Standard.Types.Enumerables;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Standard.Types.Maybe.AttemptFunctions;
using static Standard.Types.Maybe.MaybeFunctions;
using Tuple = Kagami.Library.Objects.Tuple;

namespace Kagami.Library.Runtime
{
   public class Frame
   {
      Stack<IObject> stack;
      IMaybe<IObject> returnValue;
      int address;
      Fields fields;
      Arguments arguments;
      FrameType frameType;
      bool parametersSet;

      public Frame(int address, Arguments arguments)
      {
         stack = new Stack<IObject>();
         returnValue = none<IObject>();
         this.address = address;
         fields = new Fields();
         this.arguments = arguments;
         frameType = FrameType.Function;
         parametersSet = false;
      }

      public Frame(int address, Fields fields)
      {
         stack = new Stack<IObject>();
         returnValue = none<IObject>();
         this.address = address;
         this.fields = fields;
         arguments = Arguments.Empty;
         frameType = FrameType.Function;
         parametersSet = false;
      }

      public Frame(int address, Arguments arguments, Fields fields)
      {
         stack = new Stack<IObject>();
         returnValue = none<IObject>();
         this.address = address;
         this.fields = fields;
         this.arguments = arguments;
         frameType = FrameType.Function;
         parametersSet = false;
      }

      public Frame(int address, IInvokable invokable)
      {
         stack = new Stack<IObject>();
         returnValue = none<IObject>();
         this.address = address;
         if (invokable is IProvidesFields pf && pf.ProvidesFields)
            fields = pf.Fields;
         else
            fields = new Fields();
         arguments = Arguments.Empty;
         frameType = FrameType.Function;
         parametersSet = false;
      }

      public Frame() : this(-1, Arguments.Empty) => frameType = FrameType.Standard;

      public Frame(Fields fields) : this(-1, fields) => frameType = FrameType.Standard;

      public Frame(IInvokable invokable) : this(-1, invokable) => frameType = FrameType.Standard;

      public Frame(Arguments arguments) : this(-1, arguments) => frameType = FrameType.Standard;

      public FrameType FrameType
      {
         get => frameType;
         set => frameType = value;
      }

      public void SetFields(Parameters parameters)
      {
         if (!parametersSet)
         {
            var length = Math.Min(arguments.Length, parameters.Length);
            var lastValue = Unassigned.Value;
            var lastName = "";
            var variadic = false;

            for (var i = 0; i < length && !variadic; i++)
            {
               var parameter = parameters[i];
               if (!fields.ContainsKey(parameter.Name))
                  fields.New(parameter.Name, parameter.Mutable).Force();
               lastValue = arguments[i];
               fields.Assign(parameter.Name, lastValue, true).Force();
               lastName = parameter.Name;
               variadic = parameter.Variadic;
            }

            if (variadic)
            {
               var tupleList = new List<IObject> { lastValue };
               for (var i = length; i < arguments.Length; i++)
                  tupleList.Add(arguments[i]);
               var tuple = new Tuple(tupleList.ToArray());
               fields.Assign(lastName, tuple, true).Force();
            }
            else if (length < parameters.Length)
               for (var i = length; i < parameters.Length; i++)
               {
                  var parameter = parameters[i];
                  var defaultValue = parameter.DefaultValue;
                  if (!fields.ContainsKey(parameter.Name))
                     fields.New(parameter.Name, parameter.Mutable).Force();
                  IObject value;
                  if (defaultValue.If(out var invokable))
                     value = Machine.Current.Invoke(invokable, Arguments.Empty, 0).Value;
                  else
                     value = Unassigned.Value;
                  fields.Assign(parameter.Name, value, true).Force();
               }
            else if (length < arguments.Length)
            {
               var tupleList = new List<IObject> { lastValue };
               for (var i = length; i < arguments.Length; i++)
                  tupleList.Add(arguments[i]);
               var tuple = new Tuple(tupleList.ToArray());
               fields.Assign(lastName, tuple, true).Force();
            }

            parametersSet = true;
         }
      }

      public void Push(IObject value) => stack.Push(value);

      public bool IsEmpty => stack.Count == 0;

      public IMaybe<IObject> Peek() => when(stack.Count > 0, () => stack.Peek());

      public IResult<IObject> Pop() => tryTo(() => stack.Pop());

      public void SetReturnValue(IObject value) => returnValue = value.Some();

      public IMaybe<IObject> ReturnValue => returnValue;

      public int Address
      {
         get => address;
         set => address = value;
      }

      public Fields Fields => fields;

      public Arguments Arguments => arguments;

      public void Clear() => stack.Clear();

      public override string ToString()
      {
         return (StringStream)"(" / stack.Select(v => v.Image).Listify() / ")[" / fields.FieldNames.Listify() / "]";
      }

      public void CopyFromFields(Fields sourceFields) => fields.CopyFrom(sourceFields);
   }
}