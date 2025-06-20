﻿using Core.DataStructures;
using Core.Enumerables;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Runtime;

public class Frame
{
   public static Frame TryFrame() => new() { FrameType = FrameType.Try };

   protected MaybeStack<IObject> stack = [];
   protected Maybe<IObject> returnValue = nil;
   protected Maybe<int> _address;
   protected Fields fields;
   protected Arguments arguments;
   protected FrameType frameType = FrameType.Function;
   protected bool parametersSet;

   public Frame(Maybe<int> _address, Arguments arguments)
   {
      this._address = _address;
      fields = new Fields();
      this.arguments = arguments;
   }

   public Frame(Maybe<int> _address, Fields fields)
   {
      this._address = _address;
      this.fields = fields;
      arguments = Arguments.Empty;
   }

   public Frame(Maybe<int> _address, Arguments arguments, Fields fields)
   {
      this._address = _address;
      this.fields = fields;
      this.arguments = arguments;
   }

   public Frame(Maybe<int> _address, IInvokable invokable)
   {
      this._address = _address;
      if (invokable is IProvidesFields { ProvidesFields: true } pf)
      {
         fields = pf.Fields;
      }
      else
      {
         fields = new Fields();
      }

      arguments = Arguments.Empty;
   }

   public Frame() : this(nil, Arguments.Empty) => frameType = FrameType.Standard;

   public Frame(Fields fields) : this(nil, fields) => frameType = FrameType.Standard;

   public Frame(IInvokable invokable) : this(nil, invokable) => frameType = FrameType.Standard;

   public Frame(Arguments arguments) : this(nil, arguments) => frameType = FrameType.Standard;

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
         var variadic = parameters.Length > 0 && parameters[0].Variadic;

         if (variadic)
         {
            var parameter = parameters[0];
            if (parameter.Singleton && arguments.Length == 1)
            {
               if (!fields.ContainsKey(parameter.Name))
               {
                  fields.New(parameter.Name, parameter.Mutable).Force();
               }

               if (parameter.TypeConstraint is (true, var typeConstraint) && !typeConstraint.Matches(classOf(lastValue)))
               {
                  throw incompatibleClasses(lastValue, typeConstraint.AsString);
               }

               fields.Assign(parameter.Name, arguments[0], true).Force();
            }
            else
            {
               var tuple = new KTuple([.. arguments]);
               if (!fields.ContainsKey(parameter.Name))
               {
                  fields.New(parameter.Name, parameter.Mutable).Force();
               }

               if (parameter.TypeConstraint is (true, var typeConstraint) && !typeConstraint.Matches(classOf(lastValue)))
               {
                  throw incompatibleClasses(lastValue, typeConstraint.AsString);
               }

               fields.Assign(parameter.Name, tuple, true).Force();
            }

            return;
         }

         for (var i = 0; i < length && !variadic; i++)
         {
            var parameter = parameters[i];
            lastValue = arguments[i];
            if (!fields.ContainsKey(parameter.Name))
            {
               fields.New(parameter.Name, parameter.Mutable).Force();
            }

            if (parameter.TypeConstraint is (true, var typeConstraint) && !typeConstraint.Matches(classOf(lastValue)))
            {
               throw incompatibleClasses(lastValue, typeConstraint.AsString);
            }

            fields.Assign(parameter.Name, lastValue, true).Force();
            lastName = parameter.Name;
            variadic = parameter.Variadic;
         }

         if (variadic)
         {
            List<IObject> tupleList = [lastValue];
            for (var i = length; i < arguments.Length; i++)
            {
               tupleList.Add(arguments[i]);
            }

            var tuple = new KTuple([.. tupleList]);
            fields.Assign(lastName, tuple, true).Force();
         }
         else if (length < parameters.Length)
         {
            for (var i = length; i < parameters.Length; i++)
            {
               var parameter = parameters[i];
               var _defaultValue = parameter.DefaultValue;
               if (!fields.ContainsKey(parameter.Name))
               {
                  fields.New(parameter.Name, parameter.Mutable).Force();
               }

               IObject value;
               if (_defaultValue is (true, var invokable))
               {
                  var _value = Machine.Current.Value.Invoke(invokable, Arguments.Empty);
                  if (_value is (true, var value2))
                  {
                     value = value2;
                  }
                  else if (_value.Exception is (true, var exception))
                  {
                     throw exception;
                  }
                  else
                  {
                     return;
                  }
               }
               else if (parameter.Variadic)
               {
                  value = KTuple.Empty;
                  fields.Assign(parameter.Name, value, true);
               }
               else
               {
                  value = Unassigned.Value;
               }

               if (parameter.TypeConstraint is (true, var typeConstraint) && !typeConstraint.Matches(classOf(value)))
               {
                  throw incompatibleClasses(value, typeConstraint.AsString);
               }

               fields.Assign(parameter.Name, value, true).Force();
            }
         }
         else if (length < arguments.Length)
         {
            var tupleList = new List<IObject> { lastValue };
            for (var i = length; i < arguments.Length; i++)
            {
               tupleList.Add(arguments[i]);
            }

            var tuple = new KTuple(tupleList.ToArray());
            fields.Assign(lastName, tuple, true).Force();
         }

         parametersSet = true;
      }
   }

   public void Push(IObject value) => stack.Push(value);

   public bool IsEmpty => stack.Count == 0;

   public Maybe<IObject> Peek() => stack.Peek();

   public Result<IObject> Pop() => stack.Pop().Result("Empty stack");

   public void SetReturnValue(IObject value) => returnValue = value.Some();

   public Maybe<IObject> ReturnValue => returnValue;

   public Maybe<int> Address
   {
      get => _address;
      set => _address = value;
   }

   public Fields Fields => fields;

   public Arguments Arguments => arguments;

   public void Clear() => stack.Clear();

   public override string ToString()
   {
      return (StringStream)"(" / stack.Select(v => v.Image).ToString(", ") / ")[" / fields.FieldNames.ToString(", ") / "]";
   }

   public void CopyFromFields(Fields sourceFields) => fields.CopyFrom(sourceFields);

   public Maybe<int> ErrorHandler { get; set; } = nil;

   public Maybe<Unit> Swap(int index)
   {
      var index2 = index + 1;
      if (index2 < stack.Count)
      {
         IObject[] array = [.. stack];
         (array[index], array[index2]) = (array[index2], array[index]);
         stack = [..array];

         return unit;
      }
      else
      {
         return nil;
      }
   }

   public Maybe<IObject> Pick(int index)
   {
      if (index < stack.Count)
      {
         var array = stack.ToArray();
         var item = array[index];
         var list = array.ToList();
         list.RemoveAt(index);
         list.Reverse();
         stack = [.. list];

         return item.Some();
      }
      else
      {
         return nil;
      }
   }

   public Maybe<IObject> Copy(int index)
   {
      if (index < stack.Count)
      {
         var list = stack.ToList();
         var item = list[index];
         list.Reverse();
         stack = [.. list];

         return item.Some();
      }
      else
      {
         return nil;
      }
   }

   public string AsString => frameType switch
   {
      FrameType.Standard => $"Standard frame ({stack.Count})",
      FrameType.Function => $"Function frame ({stack.Count})",
      FrameType.Try => $"Try frame ({stack.Count})",
      FrameType.Exit => $"Exit frame ({stack.Count})",
      FrameType.Skip => $"Skip frame ({stack.Count})",
      _ => "Unknown frame type"
   };

   public IEnumerable<string> AllFieldNames()
   {
      foreach (var fieldName in fields.FieldNames.Distinct())
      {
         yield return fieldName;
      }
   }
}