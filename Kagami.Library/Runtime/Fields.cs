﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Objects;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Runtime
{
   public class Fields : IEquatable<Fields>, IEnumerable<(string fieldName, Field field)>
   {
      protected const int MAX_DEPTH = 1024;

      protected Hash<string, Field> fields;
      protected AutoHash<string, List<string>> buckets;

      public Fields()
      {
         fields = new Hash<string, Field>();
         buckets = new AutoHash<string, List<string>>(_ => new List<string>(), true);
      }

      public IMatched<Field> Find(string name, bool getting, int depth = 0)
      {
         if (depth > MAX_DEPTH)
         {
            return failedMatch<Field>(exceededMaxDepth());
         }
         else if (fields.ContainsKey(name))
         {
            var field = fields[name];
            return field.Value switch
            {
               Unassigned when getting => failedMatch<Field>(fieldUnassigned(name)),
               Reference r => r.Field.Matched(),
               _ => field.Matched()
            };
         }
         else
         {
            return notMatched<Field>();
         }
      }

      public IMatched<Field> Find(Selector selector, int depth = 0)
      {
         if (depth > MAX_DEPTH)
         {
            return failedMatch<Field>(exceededMaxDepth());
         }
         else
         {
            if (fields.ContainsKey(selector.Image))
            {
               return fields[selector.Image].Matched();
            }
            else
            {
               var labelsOnlyImage = selector.LabelsOnly().Image;
               if (buckets.ContainsKey(labelsOnlyImage))
               {
                  foreach (var matchSelector in buckets[labelsOnlyImage].Where(matchSelector => selector.IsEquivalentTo((Selector)matchSelector)))
                  {
                     return fields[((Selector)matchSelector).Image].Matched();
                  }
               }

               return fields[labelsOnlyImage].Matched();
            }
         }
      }

      public bool ContainsSelector(Selector selector) => buckets.ContainsKey(selector);

      public IResult<Unit> FindByPattern(string pattern, List<Field> list, int depth = 0)
      {
         if (depth > MAX_DEPTH)
         {
            return failure<Unit>(exceededMaxDepth());
         }
         else
         {
            foreach (var key in fields.KeyArray().Where(k => k.IsMatch(pattern)))
            {
               list.Add(fields[key]);
            }

            return Unit.Success();
         }
      }

      public IResult<Field> New(string name, IObject value, bool mutable = false, bool visible = true)
      {
         return New(name, new Field { Value = value, Mutable = mutable, Visible = visible });
      }

      public IResult<Field> New(string name, bool mutable = false, bool visible = true)
      {
         return New(name, Unassigned.Value, mutable, visible);
      }

      public IResult<Field> New(string name, IMaybe<TypeConstraint> typeConstraint, bool mutable, bool visible)
      {
         return New(name, new Field
         {
            Value = Unassigned.Value, Mutable = mutable, Visible = visible, TypeConstraint = typeConstraint
         });
      }

      public IResult<Field> New(Selector selector, bool mutable = false, bool visible = true)
      {
         if (fields.ContainsKey(selector))
         {
            return failure<Field>(fieldAlreadyExists(selector));
         }
         else
         {
            var field = new Field { Value = Unassigned.Value, Mutable = mutable, Visible = visible };
            fields[selector] = field;
            buckets[selector.LabelsOnly()].Add(selector);

            return field.Success();
         }
      }

      public void New(Selector selector, IObject value, bool mutable = false, bool visible = true)
      {
         if (!fields.ContainsKey(selector))
         {
            var field = new Field { Value = value, Mutable = mutable, Visible = visible };
            fields[selector] = field;
            buckets[selector.LabelsOnly()].Add(selector);
         }
      }

      public IResult<Field> New(string name, Field field)
      {
         if (fields.ContainsKey(name))
         {
            return failure<Field>(fieldAlreadyExists(name));
         }
         else
         {
            fields[name] = field;
            return field.Success();
         }
      }

      public IResult<Field> Assign(string name, IObject value, bool overriden = false)
      {
         if (Find(name, false).ValueOrOriginal(out var field, out var original))
         {
            if (field.Mutable)
            {
               field.Value = value;
               return field.Success();
            }
            else if (field.Value is Unassigned || overriden)
            {
               field.Value = value;
               return field.Success();
            }
            else
            {
               return failure<Field>(immutableField(name));
            }
         }
         else if (original.IfNot(out var anyException) && anyException.If(out var exception))
         {
            return failure<Field>(exception);
         }
         else
         {
            return failure<Field>(fieldNotFound(name));
         }
      }

      public static IResult<Field> AssignToExisting(string name, IObject value, bool overriden = false)
      {
         if (Machine.Current.Find(name, false).ValueOrOriginal(out var field, out var original))
         {
            if (field.Mutable)
            {
               field.Value = value;
               return field.Success();
            }
            else if (field.Value is Unassigned || overriden)
            {
               field.Value = value;
               return field.Success();
            }
            else
            {
               return failure<Field>(immutableField(name));
            }
         }
         else if (original.IfNot(out var anyException) && anyException.If(out var exception))
         {
            return failure<Field>(exception);
         }
         else
         {
            return failure<Field>(fieldNotFound(name));
         }
      }

      public IResult<Field> Assign(Selector selector, bool overriden = false)
      {
         var result = Assign(selector, selector, overriden);
         if (result.If(out _))
         {
            buckets[selector.LabelsOnly().Image].Add(selector);
         }

         return result;
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
         {
            return failure<Unit>(fieldNotFound(fieldName));
         }
      }

      public IEnumerator<(string fieldName, Field field)> GetEnumerator()
      {
         foreach (var (key, value) in fields)
         {
            yield return (key, value);
         }
      }

      public void Remove(string fieldName)
      {
         if (fields.ContainsKey(fieldName))
         {
            fields.Remove(fieldName);
         }
      }

      public override string ToString() => fields.Select(i => $"{i.Key} = {i.Value.Value.Image}").ToString(", ");

      public void SetBindings(Hash<string, IObject> bindings)
      {
         foreach (var (key, value) in bindings)
         {
            if (key.IsMatch("^ ['+-']"))
            {
               var mutable = key.StartsWith("+");
               var fieldName = key.Drop(1);
               if (New(fieldName, value, mutable).IfNot(out var exception))
               {
                  throw exception;
               }
            }
            else if (AssignToExisting(key, value).IfNot(out var exception))
            {
               throw exception;
            }
         }
      }

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
            {
               return fields[name].Value;
            }
            else
            {
               throw fieldNotFound(name);
            }
         }
         set
         {
            if (fields.ContainsKey(name))
            {
               fields[name].Value = value;
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
         foreach (var (key, value) in sourceFields.fields)
         {
            fields[key] = value.Clone();
         }

         foreach (var (key, value) in sourceFields.buckets)
         {
            buckets[key] = value;
         }
      }

      public void CopyFrom(Fields sourceFields, Func<string, Field, bool> filter)
      {
         foreach (var (key, value) in sourceFields.fields.Where(i => filter(i.Key, i.Value)))
         {
            fields[key] = value.Clone();
         }

         foreach (var (key, value) in sourceFields.buckets)
         {
            buckets[key] = value;
         }
      }

      public void SetBucket(Selector selector) => buckets[selector.LabelsOnly()].Add(selector);
   }
}