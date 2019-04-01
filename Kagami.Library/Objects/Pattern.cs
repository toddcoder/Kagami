using System.Linq;
using Core.Collections;
using Core.Enumerables;

namespace Kagami.Library.Objects
{
   public struct Pattern : IObject
   {
      (IObject comparisand, Lambda lambda)[] cases;

      public Pattern((IObject, Lambda)[] cases) : this() => this.cases = cases;

      public string ClassName => "Pattern";

      public string AsString => cases.Select(c => $"{c.comparisand.AsString} = {c.lambda.AsString}").Listify(" ");

      public string Image => $"(|{cases.Select(c => $"{c.comparisand.Image} = {c.lambda.Image}").Listify("|")})";

      public int Hash => cases.GetHashCode();

      public bool IsEqualTo(IObject obj)
      {
         if (obj is Pattern pattern)
         {
            var otherCases = pattern.cases;
            if (cases.Length == otherCases.Length)
               return cases.Zip(otherCases, (c1, c2) => c1.comparisand.IsEqualTo(c2.comparisand) && c1.lambda.IsEqualTo(c2.lambda))
                  .All(b => b);
            else
               return false;
         }
         else
            return false;
      }

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => false;

      public bool IsTrue => true;

      public (IObject comparisand, Lambda lambda)[] Cases => cases;
   }
}