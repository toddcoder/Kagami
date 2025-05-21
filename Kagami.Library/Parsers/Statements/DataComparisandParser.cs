using Kagami.Library.Classes;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Parsers.Expressions;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Monads;
using Core.Monads.Lazy;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public class DataComparisandParser : StatementParser
{
   protected string className;
   protected Hash<string, (IObject[], IRangeItem)> values;
   protected IRangeItem ordinal;

   public DataComparisandParser(string className, Hash<string, (IObject[], IRangeItem)> values, IRangeItem ordinal)
   {
      this.className = className;
      this.values = values;
      this.ordinal = ordinal;
   }

   public override string Pattern => $"^ /({REGEX_CLASS}) /'('?";

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.BeginTransaction();
      var name = tokens[1].Text;
      Name = name;
      var hasArguments = tokens[2].Text == "(";
      state.Colorize(tokens, Color.Class, Color.OpenParenthesis);

      var _result =
         from possibleComparisands in getPossibleComparisands(hasArguments, className, name, state)
         from possibleOrdinal in getOrdinal(state, ordinal)
         select (possibleComparisands, possibleOrdinal);
      if (_result is (true, var (comparisands, newOrdinal)))
      {
         values[name] = (comparisands, newOrdinal);
         Ordinal = newOrdinal.Some();
         state.CommitTransaction();
         Module.Global.Value.RegisterDataComparisand(className, name);

         return unit;
      }
      else
      {
         state.RollBackTransaction();
         return _result.Exception;
      }
   }

   protected static Optional<IObject[]> getPossibleComparisands(bool hasArguments, string className, string name, ParseState state)
   {
      Module.Global.Value.ForwardReference($"{className}.{name}");
      LazyResult<Unit> _registered = nil;
      if (hasArguments)
      {
         var _result =
            from comparisandList in getComparisandList(state)
            from registered in Module.Global.Value.RegisterClass(new DataComparisandClass(className, name)).Unit
            select comparisandList;
         if (_result is (true, var comparisands))
         {
            return comparisands;
         }
         else
         {
            return _result.Exception;
         }
      }
      else if (_registered.ValueOf(Module.Global.Value.RegisterClass(new DataComparisandClass(className, name))))
      {
         return (IObject[]) [];
      }
      else
      {
         return _registered.Exception;
      }
   }

   protected static Optional<IRangeItem> getOrdinal(ParseState state, IRangeItem ordinal)
   {
      var _scan = state.Scan("^ /(|s|) /'='", Color.Whitespace, Color.Structure);
      if (_scan)
      {
         var _value = getValue(state, ExpressionFlags.Standard);
         if (_value is (true, var value))
         {
            if (value is IConstant { Object: IRangeItem ri })
            {
               return ri.Just();
            }
            else
            {
               return fail("Range item required, found {value}");
            }
         }
         else
         {
            return _value.Exception;
         }
      }
      else if (_scan.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return ordinal.Just();
      }
   }

   public string Name { get; set; } = "";

   public Maybe<IRangeItem> Ordinal { get; set; } = nil;
}