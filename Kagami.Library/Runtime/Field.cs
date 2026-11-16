using Core.Enumerables;
using Kagami.Library.Objects;
using Core.Monads;
using Kagami.Library.Classes;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Runtime;

public class Field
{
   public static Result<(IObject, Maybe<TypeConstraint>)> GetFieldValue(IObject value, Maybe<TypeConstraint> _typeConstraint)
   {
      var valueClass = classOf(value);

      if (_typeConstraint is (true, var typeConstraint and not ProtocolConstraint))
      {
         if (typeConstraint.Comparisands[0] is TupleClass { TypeConstraint: (true, var tupleTypeConstraint) } &&
             value is KTuple { TypeConstraint: (true, var valueTypeConstraint) })
         {
            var classTypes = tupleTypeConstraint.Comparisands;
            var valueTypes = valueTypeConstraint.Comparisands;
            if (classTypes.Length == valueTypes.Length)
            {
               for (var i = 0; i < classTypes.Length; i++)
               {
                  if (!classTypes[i].AssignCompatible(valueTypes[i]))
                  {
                     throw incompatibleClasses(value, $"({valueTypes.Select(c => c.Name).ToString(" * ")})",
                        $"({classTypes.Select(c => c.Name).ToString(" * ")})");
                  }
               }

               return (value, _typeConstraint);
            }
            else
            {
               throw incompatibleClasses(value, typeConstraint.AsString);
            }
         }

         if (typeConstraint.Comparisands[0] is OptionalClass { TypeConstraint: (true, var optionalTypeConstraint) })
         {
            var optionalType = optionalTypeConstraint.Comparisands[0];
            switch (value)
            {
               case Some some when some.TypeConstraint.Comparisands[0].AssignCompatible(optionalType):
               case KNil:
                  return (value, _typeConstraint);
               default:
               {
                  if (valueClass.AssignCompatible(optionalType))
                  {
                     return (new Some(value), _typeConstraint);
                  }
                  else
                  {
                     throw incompatibleClasses(value, typeConstraint.AsString);
                  }
               }
            }
         }

         if (typeConstraint.Comparisands[0] is ResultClass { TypeConstraint: (true, var resultTypeConstraint) })
         {
            var resultType = resultTypeConstraint.Comparisands[0];
            switch (value)
            {
               case Success success when success.TypeConstraint.Comparisands[0].AssignCompatible(resultType):
               case Failure:
                  return (value, _typeConstraint);
               default:
               {
                  if (valueClass.AssignCompatible(resultType))
                  {
                     return (new Success(value), _typeConstraint);
                  }
                  else if (Protocols.Protocols.Supports("PError", valueClass))
                  {
                     return (new Failure(value), _typeConstraint);
                  }
                  else
                  {
                     throw incompatibleClasses(value, typeConstraint.AsString);
                  }
               }
            }
         }

         if (typeConstraint.Matches(valueClass) || value is Placeholder)
         {
            return (value, _typeConstraint);
         }
         else
         {
            foreach (var baseClass in typeConstraint.Comparisands)
            {
               var _func = Module.AutoConversion(value.ClassName, baseClass.Name);
               if (_func is (true, var func))
               {
                  return (func(value), _typeConstraint);
               }
            }

            throw incompatibleClasses(value, typeConstraint.AsString);
         }
      }
      else
      {
         var newValue = value switch
         {
            Placeholder placeholder => placeholder,
            Any => Any.Value,
            _ => value
         };

         return (newValue, Objects.TypeConstraint.SingleType(valueClass));
      }
   }

   protected IObject value = KNil.NilValue;

   public virtual IObject Value
   {
      get => value;
      set
      {
         var _result = GetFieldValue(value, TypeConstraint);
         if (_result is (true, var (newValue, newTypeConstraint)))
         {
            this.value = newValue;
            TypeConstraint = newTypeConstraint;
         }
         else
         {
            throw _result.Exception;
         }
      }
   }

   public bool Mutable { get; set; }

   public bool Visible { get; set; } = true;

   public Maybe<TypeConstraint> TypeConstraint { get; set; } = nil;

   public bool Tolerant { get; set; }

   public required FieldType Type { get; set; }

   public Field Copy() => new()
   {
      Value = Value is ICollection collection ? collection.Copy() : Value,
      Mutable = Mutable,
      Visible = Visible,
      TypeConstraint = TypeConstraint,
      Tolerant = Tolerant,
      Type = Type
   };

   public Fields Fields { get; set; } = [];
}