using System.Linq;
using Kagami.Library.Classes;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using Standard.Types.Exceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public struct TypeConstraint : IObject
   {
      BaseClass[] comparisands;

      public TypeConstraint(BaseClass[] comparisands) : this() => this.comparisands = comparisands;

      public void RefreshClasses()
      {
         for (var i = 0; i < comparisands.Length; i++)
         {
            var comparisand = comparisands[i];
            if (comparisand is ForwardedClass forwardedClass)
               if (Module.Global.Class(forwardedClass.Name).If(out var actualClass))
                  comparisands[i] = actualClass;
               else
                  throw $"Expected {forwardedClass.Name} to exist".Throws();
         }
      }

      public string ClassName => "TypeConstraint";

      public string AsString => comparisands.Select(c => c.Name).Listify(" or ");

      public string Image => $"<{comparisands.Select(c => c.Name).Listify(" ")}>";

      public int Hash
      {
         get
         {
            var hash = 17;
            foreach (var comparisand in comparisands)
               hash += 37 * comparisand.GetHashCode();

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
                  if (otherComparisands.All(c => c.Name != comparisand.Name))
                     return false;

               return true;
            }
            else
               return false;
         }
         else
            return false;
      }

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool Matches(BaseClass baseClass) => comparisands.Any(c => c.AssignCompatible(baseClass));

      public bool IsTrue => true;
   }
}