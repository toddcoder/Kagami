using System.Collections;
using Kagami.Library.Classes;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class TypeConstraint : IObject, IEnumerable<TypeConstraint>
{
   public static TypeConstraint FromList(params string[] classNames)
   {
      return [with([.. classNames.Select(cn => Module.Global.Value.Class(cn).Required(messageClassNotFound(cn)))])];
   }

   public static TypeConstraint SingleType(BaseClass baseClass) => new([baseClass]);

   protected readonly BaseClass[] comparisands = [];

   public TypeConstraint(BaseClass[] comparisands)
   {
      this.comparisands = comparisands;
   }

   public Maybe<TypeConstraint> SubTypeConstraint { get; set; } = nil;

   public TypeConstraint Append(TypeConstraint otherTypeConstraint)
   {
      List<BaseClass> newComparisands =
      [
         .. comparisands,
         .. otherTypeConstraint.comparisands
      ];

      return [with([.. newComparisands])];
   }

   public void RefreshClasses()
   {
      for (var i = 0; i < comparisands.Length; i++)
      {
         var comparisand = comparisands[i];
         if (comparisand is ForwardedClass forwardedClass)
         {
            var _actualClass = Module.Global.Value.Class(forwardedClass.Name);
            if (_actualClass is (true, var actualClass))
            {
               comparisands[i] = actualClass;
            }
            else
            {
               throw fail($"Expected {forwardedClass.Name} to exist");
            }
         }
      }
   }

   public string ClassName => "TypeConstraint";

   public virtual string AsString => comparisands.Select(c => c.Name).ToString(" or ");

   public virtual string Image
   {
      get
      {
         if (SubTypeConstraint is (true, var subTypeConstraint))
         {
            return $"<{comparisands.Select(c => c.Name).ToString(" ")}{subTypeConstraint.Image}>";
         }
         else
         {
            return $"<{comparisands.Select(c => c.Name).ToString(" ")}>";
         }
      }
   }

   public virtual int Hash => HashCode.Combine(comparisands);

   public virtual bool IsEqualTo(IObject obj)
   {
      if (obj is TypeConstraint typeConstraint)
      {
         var otherComparisands = typeConstraint.comparisands;
         if (comparisands.Length == otherComparisands.Length)
         {
            foreach (var comparisand in comparisands)
            {
               if (otherComparisands.All(c => c.Name != comparisand.Name))
               {
                  return false;
               }
            }

            return true;
         }
         else
         {
            return false;
         }
      }
      else
      {
         return false;
      }
   }

   public virtual bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public virtual bool Matches(BaseClass baseClass)
   {
      /*
      var unassigned = classOf("Unassigned");
      if (comparisands.Any(c => c == unassigned))
      {
         return true;
      }
      */

      if (baseClass is UserClass userClass)
      {
         foreach (var comparisand in comparisands)
         {
            if (userClass.AssignCompatible(comparisand))
            {
               return true;
            }
         }

         return false;
      }
      else
      {
         return comparisands.Any(c => c.AssignCompatible(baseClass));
      }
   }

   public virtual bool Matches(UserClass userClass) => comparisands.Any(c => c.AssignCompatible(userClass));

   public virtual bool Matches(TypeConstraint typeConstraint)
   {
      return typeConstraint.comparisands.Select(Matches).Any(b => b);
   }

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();

   public TypeConstraint Merge(TypeConstraint other)
   {
      var set = new Set<string>();
      set.AddRange(comparisands.Select(bc => bc.Name));
      set.AddRange(other.comparisands.Select(bc => bc.Name));

      return FromList(set.ToArray());
   }

   public virtual TypeConstraint Equivalent()
   {
      var result = this;
      foreach (var comparisand in comparisands)
      {
         if (comparisand is IEquivalentClass equivalent)
         {
            result = result.Merge(equivalent.EquivalentTypeConstraint());
         }
      }

      return result;
   }

   public virtual IEnumerator<TypeConstraint> GetEnumerator()
   {
      foreach (var comparisand in comparisands)
      {
         yield return SingleType(comparisand);
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public virtual bool IsEquivalentTo(TypeConstraint typeConstraint)
   {
      var baseClass = comparisands[0];
      if (baseClass.Name is "Placeholder" or "Any")
      {
         return true;
      }
      else if (typeConstraint.comparisands.Contains(baseClass))
      {
         return true;
      }
      else
      {
         switch (baseClass)
         {
            case UserClass userClass:
            {
               var _parentClass = userClass.ParentClass;
               if (_parentClass is (true, var parentClass))
               {
                  var parentTypeConstraint = new TypeConstraint([parentClass]);
                  return parentTypeConstraint.IsEquivalentTo(typeConstraint);
               }
               else
               {
                  return false;
               }
            }
            case IEquivalentClass equivalentClass:
            {
               foreach (var comparisand in equivalentClass.EquivalentTypeConstraint().comparisands)
               {
                  if (typeConstraint.comparisands.Contains(comparisand))
                  {
                     return true;
                  }
               }

               return false;
            }
            default:
               return false;
         }
      }
   }

   public BaseClass[] Comparisands => comparisands;

   public virtual Maybe<IObject> ConvertToMonad(IObject value)
   {
      if (comparisands.Length > 0)
      {
         var className = comparisands[0].Name;
         return className switch
         {
            "Optional" => value switch
            {
               Some or KNil => value.Some(),
               Success success => Some.Object(success.Value).Some(),
               Failure or Error => KNil.NilValue.Some(),
               _ => Some.Object(value).Some()
            },
            "Result" => value switch
            {
               Success or Failure => value.Some(),
               Error error => new Failure(error),
               Some some => Success.Object(some.Value).Some(),
               KNil => Failure.Object("No value provided").Some(),
               _ when supportsErroring() => new Failure(value),
               _ => Success.Object(value).Some()
            },
            _ => nil
         };

         bool supportsErroring()
         {
            var erroring = Protocols.Protocols.GetOrThrow("PError");
            return erroring.Supports(value);
         }
      }

      return nil;
   }
}