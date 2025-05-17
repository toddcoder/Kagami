using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Classes;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Enumerables;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct TypeConstraint : IObject, IEnumerable<TypeConstraint>
{
   public static TypeConstraint FromList(params string[] classNames)
   {
      return new(classNames.Select(cn => Module.Global.Class(cn).Required(messageClassNotFound(cn))).ToArray());
   }

   public static TypeConstraint SingleType(BaseClass baseClass) => new([baseClass]);

   private readonly BaseClass[] comparisands;

   public TypeConstraint(BaseClass[] comparisands) : this() => this.comparisands = comparisands;

   public void RefreshClasses()
   {
      for (var i = 0; i < comparisands.Length; i++)
      {
         var comparisand = comparisands[i];
         if (comparisand is ForwardedClass forwardedClass)
         {
            var _actualClass = Module.Global.Class(forwardedClass.Name);
            if (_actualClass)
            {
               comparisands[i] = _actualClass;
            }
            else
            {
               throw fail($"Expected {forwardedClass.Name} to exist");
            }
         }
      }
   }

   public string ClassName => "TypeConstraint";

   public string AsString => comparisands.Select(c => c?.Name ?? "?").ToString(" or ");

   public string Image => $"<{comparisands.Select(c => c?.Name ?? "?").ToString(" ")}>";

   public int Hash
   {
      get
      {
         var hash = 17;
         foreach (var comparisand in comparisands)
         {
            hash += 37 * comparisand.GetHashCode();
         }

         return hash;
      }
   }

   public bool IsEqualTo(IObject obj)
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

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool Matches(BaseClass baseClass) => comparisands.Any(c => c.AssignCompatible(baseClass));

   public bool Matches(TypeConstraint typeConstraint)
   {
      var self = this;
      return typeConstraint.comparisands.Select(bc => self.Matches(bc)).Any(b => b);
   }

   public bool IsTrue => true;

   public TypeConstraint Merge(TypeConstraint other)
   {
      var set = new Set<string>();
      set.AddRange(comparisands.Select(bc => bc.Name));
      set.AddRange(other.comparisands.Select(bc => bc.Name));

      return FromList(set.ToArray());
   }

   public TypeConstraint Equivalent()
   {
      var result = this;
      foreach (var comparisand in comparisands)
      {
         if (comparisand is IEquivalentClass equivalent)
         {
            result = result.Merge(equivalent.TypeConstraint());
         }
      }

      return result;
   }

   public IEnumerator<TypeConstraint> GetEnumerator()
   {
      foreach (var comparisand in comparisands)
      {
         yield return SingleType(comparisand);
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public bool IsEquivalentTo(TypeConstraint typeConstraint)
   {
      var baseClass = comparisands[0];
      if (typeConstraint.comparisands.Contains(baseClass))
      {
         return true;
      }
      else if (baseClass is IEquivalentClass equivalentClass)
      {
         foreach (var comparisand in equivalentClass.TypeConstraint().comparisands)
         {
            if (typeConstraint.comparisands.Contains(comparisand))
            {
               return true;
            }
         }

         return false;
      }
      else
      {
         return false;
      }
   }
}