using Kagami.Library.Objects;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Runtime;

public class Field
{
   protected IObject value = KNil.NilValue;

   public virtual IObject Value
   {
      get => value;
      set
      {
         var valueClass = classOf(value);

         if (TypeConstraint is (true, var typeConstraint))
         {
            if (convertToMonad(typeConstraint.Comparisands[0].Name, value) is (true, var monad))
            {
               this.value = monad;
               return;
            }

            if (typeConstraint.Matches(valueClass) || value is Placeholder)
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
            this.value = value switch
            {
               Placeholder placeholder => placeholder,
               Any => Any.Value,
               _ => value
            };

            TypeConstraint = Objects.TypeConstraint.SingleType(valueClass);
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