using Kagami.Library.Objects;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Runtime;

public class Field
{
   protected IObject value;

   public IObject Value
   {
      get => value;
      set
      {
         var valueClass = classOf(value);
         if (TypeConstraint is (true, var typeConstraint))
         {
            if (typeConstraint.Matches(valueClass))
            {
               if (this.value is Reference reference)
               {
                  reference.Field.Value = value;
               }
               else
               {
                  this.value = value;
               }
            }
            else
            {
               throw incompatibleClasses(value, typeConstraint.AsString);
            }
         }
         else
         {
            if (this.value is Reference reference)
            {
               reference.Field.Value = value;
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

   public Field Clone() => new()
   {
      Value = Value,
      Mutable = Mutable,
      Visible = Visible
   };

   public Fields Fields { get; set; }
}