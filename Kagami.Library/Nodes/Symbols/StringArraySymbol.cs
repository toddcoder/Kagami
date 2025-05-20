using Core.Matching;
using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class StringArraySymbol : Symbol
{
   protected KArray array;

   public StringArraySymbol(string source) => array = new KArray(source.Trim().Unjoin("/s+").Select(KString.StringObject));

   public override void Generate(OperationsBuilder builder) => builder.PushObject(array);

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => array.Image;
}