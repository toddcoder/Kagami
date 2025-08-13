using Kagami.Library.Objects;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Runtime;

public class Field
{
   protected IObject value = KNil.NilValue;

   public IObject Value
   {
      get
      {
         if (value is Lazy lazy)
         {
            value = lazy.Value;
            TypeConstraint = Objects.TypeConstraint.SingleType(classOf(value));

            Value = value;
         }

         return value;
      }
      set
      {
         var valueClass = classOf(value);
         if (TypeConstraint is (true, var typeConstraint))
         {
            if (typeConstraint.Matches(valueClass))
            {
               this.value = value;
               if (OriginalField is (true, var originalField))
               {
                  originalField.value = value;
               }
            }
            else if (value is Placeholder)
            {
               this.value = value;
            }
            else
            {
               foreach (var baseClass in typeConstraint.Comparisands)
               {
                  var _func = Module.AutoConversion(value.ClassName, baseClass.Name);
                  if (_func is (true, var func))
                  {
                     this.value = func(value);
                     return;
                  }
               }

               throw incompatibleClasses(value, typeConstraint.AsString);
            }
         }
         else
         {
            if (OriginalField is (true, var originalField))
            {
               originalField.value = value;
            }

            if (value is Placeholder placeholder)
            {
               this.value = placeholder;
            }
            else
            {
               this.value = value;
            }

            TypeConstraint = Objects.TypeConstraint.SingleType(valueClass);
         }
      }
   }

   public bool Mutable { get; set; }

   public bool Visible { get; set; } = true;

   public Maybe<TypeConstraint> TypeConstraint { get; set; } = nil;

   public bool Tolerant { get; set; }

   public required FieldType Type { get; set; }

   public Field Clone() => new()
   {
      Value = Value,
      Mutable = Mutable,
      Visible = Visible,
      TypeConstraint = TypeConstraint,
      Tolerant = Tolerant,
      Type = Type
   };

   public Fields Fields { get; set; } = [];

   public Maybe<Field> OriginalField { get; set; } = nil;
}