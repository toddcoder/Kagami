using System.Collections.Generic;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;
using DataType = Kagami.Library.Nodes.Statements.DataType;

namespace Kagami.Library.Parsers.Statements;

public class DataTypeParser : StatementParser
{
   public override string Pattern => $"^ /'type' /(/s+) /({REGEX_CLASS}) {REGEX_ANTICIPATE_END}";

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var className = tokens[3].Text;
      state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class);

      var values = new Hash<string, (IObject[], IRangeItem)>();
      var dataComparisandNames = new List<string>();
      var dataTypeClass = new DataTypeClass(className);
      var _registerClass = Module.Global.RegisterClass(dataTypeClass);
      if (_registerClass)
      {
         Module.Global.ForwardReference(className);
         IRangeItem ordinal = (Int)0;

         var _advance = state.Advance();
         if (_advance)
         {
            while (state.More)
            {
               var parser = new DataComparisandParser(className, values, ordinal);
               var _scan = parser.Scan(state);
               if (_scan)
               {
                  var _registerDataComparisand = dataTypeClass.RegisterDataComparisand(parser.Name, (IObject)parser.Ordinal);
                  if (_registerDataComparisand)
                  {
                     dataComparisandNames.Add(parser.Name);
                     ordinal = parser.Ordinal.Successor;
                  }
                  else
                  {
                     return _registerDataComparisand.Exception;
                  }
               }
               else if (_scan.Exception is (true, var exception))
               {
                  return exception;
               }
               else
               {
                  break;
               }
            }

            state.Regress();
         }
         else
         {
            return _advance.Exception;
         }

         state.AddStatement(new DataType(className, values.ToHash(i => i.Key, i =>
         {
            var (data, rangeItem) = i.Value;
            return (data, (IObject)rangeItem);
         })));

         return unit;
      }
      else
      {
         return _registerClass.Exception;
      }
   }
}