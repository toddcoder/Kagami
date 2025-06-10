using System.Text.RegularExpressions;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;
using DataType = Kagami.Library.Nodes.Statements.DataType;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class DataTypeParser : StatementParser
{

   [GeneratedRegex($@"^(\s*)(data)(\s+)({REGEX_CLASS})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var className = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class);

      Hash<string, (IObject[], IRangeItem)> values = [];
      List<string> dataComparisandNames = [];
      var dataTypeClass = new DataTypeClass(className);
      var _registerClass = Module.Global.Value.RegisterClass(dataTypeClass);
      if (_registerClass)
      {
         Module.Global.Value.ForwardReference(className);
         IRangeItem ordinal = (Int)0;

         var _result = state.BeginBlock();
         if (_result)
         {
            while (state.More)
            {
               var parser = new DataComparisandParser(className, values, ordinal);
               var _scan = parser.Scan(state);
               if (_scan)
               {
                  var _possibleOrdinal = parser.Ordinal.Result("Serious error: ordinal not provided");
                  if (_possibleOrdinal is (true, var possibleOrdinal))
                  {
                     ordinal = possibleOrdinal;
                  }
                  else
                  {
                     return _possibleOrdinal.Exception;
                  }

                  var _registerDataComparisand = dataTypeClass.RegisterDataComparisand(parser.Name, (IObject)ordinal);
                  if (_registerDataComparisand)
                  {
                     dataComparisandNames.Add(parser.Name);
                     ordinal = ordinal.Successor;
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

            _result = state.EndBlock();
            if (!_result)
            {
               return _result.Exception;
            }
         }
         else
         {
            return _result.Exception;
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