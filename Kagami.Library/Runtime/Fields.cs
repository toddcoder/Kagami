using Core.Collections;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Monads.Lazy;
using Core.Strings;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using System.Collections;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Runtime;

public class Fields : IEquatable<Fields>, IEnumerable<(string fieldName, Field field)>
{
   protected const int MAX_DEPTH = 1024;

   protected Hash<string, Field> fields = new();
   protected Memo<string, List<string>> buckets = new Memo<string, List<string>>.Function(_ => []);

   public Optional<Field> Find(string name, bool getting, int depth = 0)
   {
      if (depth > MAX_DEPTH)
      {
         return exceededMaxDepth();
      }
      else if (fields.Maybe[name] is (true, var field))
      {
         return field.Value switch
         {
            Reference r => r.Field,
            Unassigned when getting => fieldUnassigned(name),
            _ => field
         };
      }
      else
      {
         return nil;
      }
   }

   public Optional<Field> Find(Selector selector, int depth = 0)
   {
      if (depth > MAX_DEPTH)
      {
         return exceededMaxDepth();
      }
      else
      {
         if (fields.Maybe[selector.Image] is (true, var field))
         {
            return field;
         }
         else
         {
            var labelsOnlyImage = selector.LabelsOnly().Image;
            foreach (var bucket in buckets[labelsOnlyImage]
                        .Where(matchSelector => selector.IsEquivalentTo((Selector)matchSelector)))
            {
               Selector matchSelector = bucket;
               if (selector.IsEquivalentTo(matchSelector))
               {
                  return fields.Maybe[bucket].Optional();
               }
            }

            return fields.Maybe[labelsOnlyImage].Optional();
         }
      }
   }

   public bool ContainsSelector(Selector selector) => buckets.ContainsKey(selector);

   public Result<Unit> FindByPattern(string pattern, List<Field> list, int depth = 0)
   {
      if (depth > MAX_DEPTH)
      {
         return exceededMaxDepth();
      }
      else
      {
         foreach (var key in fields.KeyArray().Where(k => k.IsMatch(pattern)))
         {
            list.Add(fields[key]);
         }

         return unit;
      }
   }

   public Result<Field> New(string name, FieldType type, IObject value, bool mutable = false, bool visible = true)
   {
      return New(name, new Field { Value = value, Mutable = mutable, Visible = visible, Type = type });
   }

   public Result<Field> New(string name, FieldType type, bool mutable = false, bool visible = true)
   {
      return New(name, type, Unassigned.Value, mutable, visible);
   }

   public Result<Field> New(string name, FieldType type, Maybe<TypeConstraint> typeConstraint, bool mutable, bool visible)
   {
      return New(name, new Field
      {
         Value = Unassigned.Value, Mutable = mutable, Visible = visible, TypeConstraint = typeConstraint, Type = type
      });
   }

   public Result<Field> NewGuarded(string name, FieldType type, Maybe<TypeConstraint> typeConstraint, Lambda predicate, Maybe<IObject> _failure,
      bool mutable = false, bool visible = true)
   {
      var field = new GuardedField(name, predicate, _failure)
         { Value = Unassigned.Value, Mutable = mutable, Visible = visible, TypeConstraint = typeConstraint, Type = type };
      if (fields.ContainsKey(name) && !field.Tolerant)
      {
         return fieldAlreadyExists(name);
      }
      else
      {
         fields[name] = field;
         return field;
      }
   }

   public Result<Field> New(string name, FieldType type, Maybe<TypeConstraint> typeConstraint, IObject value, bool mutable, bool visible)
   {
      return New(name, new Field
      {
         Value = value,
         Mutable = mutable,
         Visible = visible,
         TypeConstraint = typeConstraint,
         Type = type
      });
   }

   public Result<Field> NewOrAssign(string name, FieldType type, IObject value, bool mutable = false, bool visible = true)
   {
      if (fields.Maybe[name] is (true, var field))
      {
         field.Value = value;
         return field;
      }
      else
      {
         return New(name, type, value, mutable, visible);
      }
   }

   public Result<Field> NewRefField(Field originalField, string name, FieldType type, Maybe<TypeConstraint> typeConstraint, bool mutable,
      bool visible)
   {
      var refField = new RefField(originalField)
         { Type = type, Value = originalField.Value, Mutable = mutable, TypeConstraint = typeConstraint, Visible = visible };
      return New(name, refField);
   }

   public bool SelectorExists(Selector selector) => fields.Maybe[selector];

   public Result<Field> NewSelector(Selector selector, FieldType type, bool mutable = false, bool visible = true, bool overriding = false)
   {
      if (fields.Maybe[selector] is (true, var foundField))
      {
         if (foundField.Tolerant || overriding)
         {
            return foundField;
         }
         else
         {
            return fieldAlreadyExists(selector);
         }
      }
      else
      {
         var field = new Field { Value = Unassigned.Value, Mutable = mutable, Visible = visible, Type = type };
         fields[selector] = field;
         buckets[selector.LabelsOnly()].Add(selector);

         return field;
      }
   }

   public Result<Field> NewSelector(Selector selector, FieldType type, IObject value, bool mutable = false, bool visible = true,
      bool overriding = false)
   {
      if (fields.Maybe[selector] is (true, var foundField))
      {
         if (foundField.Tolerant || overriding)
         {
            foundField.Value = value;
            return foundField;
         }
         else
         {
            return fieldAlreadyExists(selector);
         }
      }
      else
      {
         var field = new Field { Value = value, Mutable = mutable, Visible = visible, Type = type };
         fields[selector] = field;
         buckets[selector.LabelsOnly()].Add(selector);

         return field;
      }
   }

   public Result<Field> New(string name, Field field)
   {
      if (fields.ContainsKey(name) && !field.Tolerant)
      {
         return fieldAlreadyExists(name);
      }
      else
      {
         fields[name] = field;
         return field;
      }
   }

   public Result<Field> Assign(string name, IObject value, bool overriden = false, bool isReference = false)
   {
      var _field = Find(name, false);
      if (_field is (true, var field))
      {
         if (isReference)
         {
            if (Module.Global.Value.RetrievedFields.Maybe[value.Id] is (true, var fieldName))
            {
               if (Machine.Current.Find(fieldName, true) is (true,
                   { Mutable: true } originalField))
               {
                  var reference = new Reference(originalField);
                  field.Value = reference;
                  return field;
               }
               else
               {
                  return immutableField(fieldName);
               }
            }
            else
            {
               return mustUseVariable();
            }
         }

         if (field.Mutable || field.Value is Unassigned || overriden)
         {
            field.Value = value;
            return field;
         }
         else
         {
            return immutableField(name);
         }
      }
      else if (_field.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return fieldNotFound(name);
      }
   }

   public Result<Field> AssignParameter(Parameter parameter, IObject value)
   {
      if (parameter.TypeConstraint is (true, var typeConstraint))
      {
         if (typeConstraint.ConvertToMonad(value) is (true, var monad))
         {
            value = monad;
         }
         else if (!typeConstraint.Matches(classOf(value)))
         {
            return incompatibleClasses(value, typeConstraint.AsString);
         }
      }

      if (parameter.Reference)
      {
         if (Module.Global.Value.RetrievedFields.Maybe[value.Id] is (true, var fieldName))
         {
            if (Machine.Current.Find(fieldName, true) is (true,
                { Mutable: true } originalField))
            {
               Remove(parameter.Name);
               var _field = NewRefField(originalField, parameter.Name, FieldType.Parameter, parameter.TypeConstraint, parameter.Mutable, true);
               if (_field is (true, var field))
               {
                  return field;
               }
               else
               {
                  return _field;
               }
            }
            else
            {
               return immutableField(fieldName);
            }
         }
         else
         {
            return mustUseVariable();
         }
      }
      else if (parameter.Lazy)
      {
         Remove(parameter.Name);
         var _field = New(parameter.Name, FieldType.Parameter, parameter.TypeConstraint, parameter.Mutable, true);
         if (_field is (true, var field))
         {
            var oldLazyName = ((SymbolObject)value).AsString;
            var _oldLazyField = Machine.Current.Find(oldLazyName, true);
            if (_oldLazyField is (true, var oldLazyField))
            {
               var newLazyName = lazyName(parameter.Name);
               fields[newLazyName] = oldLazyField.Copy();
               field.Value = new Singleton();
            }

            return field;
         }
         else
         {
            return _field;
         }
      }
      else
      {
         Remove(parameter.Name);
         var _field = New(parameter.Name, FieldType.Parameter, parameter.TypeConstraint, parameter.Mutable, true);
         if (_field is (true, var field))
         {
            field.Value = value;
            return field;
         }
         else
         {
            return _field;
         }
      }
   }

   public Result<Field> AssignLocal(string name, FieldType type, IObject value, bool overriden = false)
   {
      var _field = Find(name, false);
      if (_field is (true, var field))
      {
         field.Value = value;
         fields[name] = new Field { Value = value, Mutable = true, Visible = true, Type = type };

         return field;
      }
      else if (_field.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         var newField = new Field { Value = value, Mutable = true, Type = type };
         fields[name] = newField;

         return newField;
      }
   }

   public Result<Field> AssignToExisting(string name, IObject value, bool overriden = false)
   {
      var _field = Machine.Current.Find(name, false);
      if (_field is (true, var field))
      {
         if (field.Mutable || field.Value is Unassigned || overriden)
         {
            field.Value = value;
            return field;
         }
         else
         {
            return immutableField(name);
         }
      }
      else if (_field.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return fieldNotFound(name);
      }
   }

   public Result<Field> Assign(Selector selector, bool overriden = false)
   {
      var _result = Assign(selector, selector, overriden);
      if (_result)
      {
         buckets[selector.LabelsOnly().Image].Add(selector);
      }

      return _result;
   }

   public Result<IObject> GetFieldValue(string fieldName)
   {
      return fields.Require(fieldName, messageFieldNotFound(fieldName)).Map(f => f.Value);
   }

   public Result<Unit> SetFieldValue(string fieldName, IObject value)
   {
      if (fields.Maybe[fieldName] is (true, var field))
      {
         field.Value = value;
         return unit;
      }
      else
      {
         return fieldNotFound(fieldName);
      }
   }

   public IEnumerator<(string fieldName, Field field)> GetEnumerator()
   {
      foreach (var (key, value) in fields)
      {
         yield return (key, value);
      }
   }

   public void Remove(string fieldName) => fields.Remove(fieldName);

   public override string ToString() => fields.Select(i => $"{i.Key} = {i.Value.Value.Image}").ToString(", ");

   public void SetBindings(Hash<string, IObject> bindings)
   {
      foreach (var (key, value) in bindings)
      {
         LazyResult<Field> _assignedField = nil;
         if (key.IsMatch("^ ['+-']"))
         {
            var fieldName = key.Drop(1);
            var _field = Find(fieldName, true);
            if (_field is (true, var field))
            {
               field.Value = value;
            }
            else
            {
               _field = New(fieldName, FieldType.Binding, value, true).Optional();
               if (!_field)
               {
                  throw _field.Exception;
               }
            }
         }
         else if (!_assignedField.ValueOf(AssignToExisting(key, value)))
         {
            throw _assignedField.Exception;
         }
      }
   }

   public bool Equals(Fields? other)
   {
      return other is not null && fields.Count == other.fields.Count &&
         fields.Select(i => i.Value.Value.IsEqualTo(other.fields[i.Key].Value)).All(b => b);
   }

   public override bool Equals(object? obj) => obj is Fields f && Equals(f);

   public override int GetHashCode() => fields.GetHashCode();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public bool ContainsKey(string fieldName) => fields.ContainsKey(fieldName);

   public IObject this[string name]
   {
      get
      {
         if (fields.Maybe[name] is (true, var field))
         {
            return field.Value;
         }
         else
         {
            throw fieldNotFound(name);
         }
      }
      set
      {
         if (fields.Maybe[name] is (true, var field))
         {
            field.Value = value;
         }
         else
         {
            throw fieldNotFound(name);
         }
      }
   }

   public int Length => fields.Count;

   public string[] FieldNames => fields.KeyArray();

   public void CopyFrom(Fields sourceFields)
   {
      StringSet keysToOmit = [..sourceFields.fields.Where(i => i.Value.Value.ClassName is "Lambda").Select(i => i.Key)];
      foreach (var (key, value) in sourceFields.fields.Where(i => !keysToOmit.Contains(i.Key)))
      {
         fields[key] = value.Copy();
      }

      foreach (var (key, value) in sourceFields.buckets.Where(i => !keysToOmit.Contains(i.Key)))
      {
         buckets[key] = value;
      }
   }

   public void CopyFrom(Fields sourceFields, Func<string, Field, bool> filter)
   {
      foreach (var (key, value) in sourceFields.fields.Where(i => filter(i.Key, i.Value)))
      {
         fields[key] = value.Copy();
      }

      foreach (var (key, value) in sourceFields.buckets)
      {
         buckets[key] = value;
      }
   }

   public void SetBucket(Selector selector) => buckets[selector.LabelsOnly()].Add(selector);

   public Fields Clone()
   {
      Hash<string, Field> newFields = [];
      foreach (var (fieldName, field) in fields)
      {
         newFields[fieldName] = field.Copy();
      }

      Memo<string, List<string>> newBuckets = new Memo<string, List<string>>.Function(_ => []);
      foreach (var (key, value) in buckets)
      {
         newBuckets[key] = value;
      }

      return new Fields
      {
         fields = newFields,
         buckets = newBuckets
      };
   }

   public string AsString => fields.Select(i => $"{i.Key}({i.Value.Value.ClassName})").ToString(", ");

   public Memo<string, List<string>> Buckets => buckets;

   public void Remove(Func<FieldType, bool> predicate)
   {
      var fieldsToDelete = fields.Where(i => predicate(i.Value.Type)).Select(i => i.Key);
      foreach (var fieldName in fieldsToDelete)
      {
         Remove(fieldName);
      }
   }
}