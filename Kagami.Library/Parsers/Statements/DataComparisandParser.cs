﻿using Kagami.Library.Classes;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Parsers.Expressions;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
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

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         state.BeginTransaction();
         var name = tokens[1].Text;
         Name = name;
         var hasArguments = tokens[2].Text == "(";
         state.Colorize(tokens, Color.Class, Color.OpenParenthesis);

         var result =
            from possibleComparisands in getPossibleComparisands(hasArguments, className, name, state)
            from possibleOrdinal in getOrdinal(state, ordinal)
            select (possibleComparisands, possibleOrdinal);
         if (result.ValueOrCast<Unit>(out var item, out var asUnit))
         {
            var (comparisands, newOrdinal) = item;
            values[name] = (comparisands, newOrdinal);
            Ordinal = newOrdinal;
            state.CommitTransaction();
            Module.Global.RegisterDataComparisand(className, name);

            return Unit.Matched();
         }
         else
         {
            state.RollBackTransaction();
            return asUnit;
         }
      }

      protected static IMatched<IObject[]> getPossibleComparisands(bool hasArguments, string className, string name, ParseState state)
      {
         Module.Global.ForwardReference($"{className}.{name}");
         if (hasArguments)
         {
            var result =
               from comparisandList in getComparisandList(state)
               from registered in Module.Global.RegisterClass(new DataComparisandClass(className, name))
                  .Map(_ => Unit.Matched()).Recover(failedMatch<Unit>)
               select comparisandList;
            if (result.ValueOrOriginal(out var comparisands, out var original))
            {
               return comparisands.Matched();
            }
            else
            {
               return original;
            }
         }
         else if (Module.Global.RegisterClass(new DataComparisandClass(className, name)).If(out _, out var exception))
         {
            return new IObject[0].Matched();
         }
         else
         {
            return failedMatch<IObject[]>(exception);
         }
      }

      protected static IMatched<IRangeItem> getOrdinal(ParseState state, IRangeItem ordinal)
      {
         if (state.Scan("^ /(|s|) /'='", Color.Whitespace, Color.Structure).If(out _, out var anyException))
         {
            if (getValue(state, ExpressionFlags.Standard).ValueOrCast<IRangeItem>(out var value, out var asIRangeItem))
            {
               if (value is IConstant { Object: IRangeItem ri })
               {
                  return ri.Matched();
               }
               else
               {
                  return $"Range item required, found {value}".FailedMatch<IRangeItem>();
               }
            }
            else
            {
               return asIRangeItem;
            }
         }
         else if (anyException.If(out var exception))
         {
            return failedMatch<IRangeItem>(exception);
         }
         else
         {
            return ordinal.Matched();
         }
      }

      public string Name { get; set; }

      public IRangeItem Ordinal { get; set; }
   }
}