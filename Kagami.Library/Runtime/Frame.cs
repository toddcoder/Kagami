using Core.DataStructures;
using Core.Enumerables;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;

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

   public void CopyFields(Fields fields)
   {
      foreach (var (fieldName, field) in fields)
      {
         var _field = this.fields.Find(fieldName);
         if (_field is (true, var existingField))
         {
            existingField.Value = field.Value;
         }
         else
         {
            this.fields.New(fieldName, field.Type, field.Mutable);
            this.fields.Assign(fieldName, field.Value, true);
         }
      }
   }

   public void SetFields(Parameters parameters)
   {
      if (!parametersSet)
      {
         var length = Math.Min(arguments.Length, parameters.Length);
         var lastValue = Unassigned.Value;
         var variadic = parameters.Length > 0 && parameters[0].Variadic;

         if (variadic)
         {
            var parameter = parameters[0];
            if (parameter.Singleton && arguments.Length == 1)
            {
               fields.AssignParameter(parameter, arguments[0]).Force();
            }
            else
            {
               var array = new KArray([.. arguments]);
               fields.AssignParameter(parameter,array).Force();
            }

            return;
         }

         for (var i = 0; i < length && !variadic; i++)
         {
            var parameter = parameters[i];
            if (parameter.NoCapturing)
            {
               continue;
            }

            lastValue = arguments[i];
            fields.AssignParameter(parameter, lastValue);
            variadic = parameter.Variadic;
         }

         if (variadic)
         {
            List<IObject> list = [.. getValueAsEnumerable(lastValue)];
            for (var i = length; i < arguments.Length; i++)
            {
               list.AddRange(getValueAsEnumerable(arguments[i]));
            }

            var array = new KArray([.. list]);
            fields.AssignParameter(parameters[^1], array).Force();
         }
         else if (length < parameters.Length)
         {
            for (var i = length; i < parameters.Length; i++)
            {
               var parameter = parameters[i];
               if (parameter.NoCapturing)
               {
                  continue;
               }

               var _defaultValue = parameter.DefaultValue;
               IObject value;
               if (_defaultValue is (true, var invokable))
               {
                  var _value = Machine.Current.Value.Invoke(invokable, Arguments.Empty, nil);
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
                  value = KArray.Empty;
                  fields.AssignParameter(parameter, value).Force();
               }
               else
               {
                  value = Unassigned.Value;
               }

               fields.AssignParameter(parameter, value);
            }
         }
         else if (length < arguments.Length)
         {
            List<IObject> list = [.. getValueAsEnumerable(lastValue)];
            for (var i = length; i < arguments.Length; i++)
            {
               list.AddRange(getValueAsEnumerable(arguments[i]));
            }

            var array = new KArray(list);
            fields.AssignParameter(parameters[^1], array).Force();
         }

         parametersSet = true;
      }

      return;

      IEnumerable<IObject> getValueAsEnumerable(IObject value)
      {
         if (value is ICollection { ExpandForArray: true } collection)
         {
            var iterator = collection.GetIterator(false);
            foreach (var item in iterator.List())
            {
               yield return item;
            }
         }
         else
         {
            yield return value;
         }
      }
   }

   public void SetFields(Fields fields)
   {
      foreach (var (fieldName, field) in fields.Where(f =>
                  f.field.Type != FieldType.Parameter && f.field.Type != FieldType.Binding && f.field.Type != FieldType.Assignment))
      {
         this.fields.AssignLocal(fieldName, field.Type, field.Value, true).Force();
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
      return (StringStream)"(" / stack.Select(v => v.Image).ToString(", ") / ")[" / fields.AsString / "]";
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

   public Maybe<Lambda> Lambda { get; set; } = nil;
}