using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Objects;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Runtime
{
   public class Fields : IEquatable<Fields>, IEnumerable<(string fieldName, Field field)>
   {
      const int MAX_DEPTH = 1024;

      Hash<string, Field> fields;

      public Fields() => fields = new Hash<string, Field>();

      public IMatched<Field> Find(string name, bool getting, int depth = 0)
      {
         if (depth > MAX_DEPTH)
            return "Exceeded max depth".FailedMatch<Field>();
         else if (fields.ContainsKey(name))
         {
            var field = fields[name];
            if (field.Value is Unassigned && getting)
               return failedMatch<Field>(fieldUnassigned(name));
            else
               return field.Matched();
         }
         else
            return notMatched<Field>();
      }

      public IResult<Field> New(string name, IObject value, bool mutable = false, bool visible = true)
      {
         return New(name, new Field { Value = value, Mutable = mutable, Visible = visible });
      }

      public IResult<Field> New(string name, bool mutable = false, bool visible = true)
      {
         return New(name, Unassigned.Value, mutable, visible);
      }

      public IResult<Field> New(string name, Field field)
      {
         if (fields.ContainsKey(name))
            return $"Field {name} already exists".Failure<Field>();
         else
         {
            fields[name] = field;
            return field.Success();
         }
      }

      public IResult<Field> Assign(string name, IObject value, bool overriden = false)
      {
         if (Find(name, false).If(out var field, out var original))
            if (field.Mutable)
            {
               if (field.Value is Unassigned || classOf(field.Value).AssignCompatible(classOf(value)))
               {
                  field.Value = value;
                  return field.Success();
               }
               else
                  return failure<Field>(incompatibleClasses(value, field.Value.ClassName));
            }
            else if (field.Value is Unassigned || overriden)
            {
               field.Value = value;
               return field.Success();
            }
            else
               return failure<Field>(immutableField(name));
         else if (original.IsFailedMatch)
            return failure<Field>(original.Exception);
         else
            return failure<Field>(fieldNotFound(name));
      }

      public IResult<IObject> GetFieldValue(string fieldName)
      {
         return fields.Require(fieldName, messageFieldNotFound(fieldName)).Map(f => f.Value);
      }

      public IResult<Unit> SetFieldValue(string fieldName, IObject value)
      {
         if (fields.ContainsKey(fieldName))
         {
            fields[fieldName].Value = value;
            return Unit.Success();
         }
         else
            return failure<Unit>(fieldNotFound(fieldName));
      }

      public IEnumerator<(string fieldName, Field field)> GetEnumerator()
      {
         foreach (var item in fields)
            yield return (item.Key, item.Value);
      }

      public override string ToString() => fields.Select(i => $"{i.Key} = {i.Value.Value.Image}").Listify();

      public void SetBindings(Hash<string, IObject> bindings, bool mutable, bool strict)
      {
         foreach (var binding in bindings)
            if (fields.ContainsKey(binding.Key))
            {
               if (strict)
                  throw immutableField(binding.Key);
               Assign(binding.Key, binding.Value);
            }
            else
               New(binding.Key, binding.Value, mutable);
      }

/*      public void AssignBindings(Hash<string, IObject> bindings, bool mutable)
      {
         foreach (var binding in bindings)
            if (Find(binding.Key, true).If(out var field, out var isNotMatched, out var exception))
               if (field.Mutable)
                  Assign(binding.Key, binding.Value);
               else
                  throw immutableField(binding.Key);
            else if (isNotMatched)
               New(binding.Key, binding.Value, mutable);
            else
               throw exception;
      }*/

      public bool Equals(Fields other)
      {
         return fields.Count == other.fields.Count &&
            fields.Select(i => i.Value.Value.IsEqualTo(other.fields[i.Key].Value)).All(b => b);
      }

      public override bool Equals(object obj) => obj is Fields f && Equals(f);

      public override int GetHashCode() => fields.GetHashCode();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public bool ContainsKey(string fieldName) => fields.ContainsKey(fieldName);

      public IObject this[string name]
      {
         get
         {
            if (fields.ContainsKey(name))
               return fields[name].Value;
            else
               throw fieldNotFound(name);
         }
         set
         {
            if (fields.ContainsKey(name))
               fields[name].Value = value;
            else
               throw fieldNotFound(name);
         }
      }

      public int Length => fields.Count;

      public string[] FieldNames => fields.KeyArray();

      public void CopyFrom(Fields sourceFields)
      {
         foreach (var item in sourceFields.fields)
            fields[item.Key] = item.Value.Clone();
      }
   }
}