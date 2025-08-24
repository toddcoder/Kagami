using Core.Matching;
using Core.Strings;
using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class StringArraySymbol : Symbol
{
   protected KArray kArray;

   public StringArraySymbol(string source)
   {
      kArray = source.IsEmpty() ? KArray.Empty : new KArray(source.Trim().Unjoin("/s+").Select(KString.StringObject));
   }

   public override void Generate(OperationsBuilder builder) => builder.PushObject(kArray);

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => kArray.Image;
}