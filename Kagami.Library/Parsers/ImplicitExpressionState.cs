using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers;

public class ImplicitExpressionState
{
   public string FieldName1 { get; set; } = "";

   public string FieldName2 { get; set; } = "";

   public Maybe<Symbol> Symbol1 { get; set; } = nil;

   public Maybe<Symbol> Symbol2 { get; set; } = nil;
}