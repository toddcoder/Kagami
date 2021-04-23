using System.Linq;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Core.RegularExpressions;

namespace Kagami.Library.Nodes.Symbols
{
   public class StringArraySymbol : Symbol
   {
      protected Array array;

      public StringArraySymbol(string source) => array = new Array(source.Trim().Split("/s+").Select(String.StringObject));

      public override void Generate(OperationsBuilder builder) => builder.PushObject(array);

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => array.Image;
   }
}