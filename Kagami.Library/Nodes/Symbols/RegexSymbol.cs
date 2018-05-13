using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class RegexSymbol : Symbol, IConstant
   {
      Regex regex;

      public RegexSymbol(string pattern, bool ignoreCase, bool multiline, bool global)
      {
         regex = new Regex(pattern, ignoreCase, multiline, global);
      }

      public override void Generate(OperationsBuilder builder) => builder.PushObject(regex);

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => regex.Image;

      public IObject Object => regex;
   }
}