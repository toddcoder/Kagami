using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers;

public class ImplicitState
{
   public ImplicitState(Symbol symbol, string message, int parameterCount, string fieldName)
   {
      Symbol = symbol;
      Message = message;
      ParameterCount = parameterCount;
      FieldName = fieldName;
   }

   public Symbol Symbol { get; }

   public string Message { get; }

   public int ParameterCount { get; }

   public string FieldName { get; }

   public Maybe<Symbol> Two { get; set; } = nil;
}