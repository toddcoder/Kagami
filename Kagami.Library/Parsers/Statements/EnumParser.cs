using System.Text.RegularExpressions;
using Core.Matching;
using Core.Monads;
using Kagami.Library.Classes;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Class = Kagami.Library.Nodes.Statements.Class;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class EnumParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(enum)(\s+)({REGEX_CLASS})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var className = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class);

      Module.Global.Value.ForwardReference(className);

      var builder = new EnumClassBuilder(className);
      var _registered = builder.Register();
      if (_registered)
      {
         var cls = new Class(builder);
         state.AddStatement(cls);

         var userClass = (EnumClass)builder.UserClass;

         var _beginBlock = state.BeginBlock();
         if (_beginBlock)
         {
            var commonBlock = new Block();
            var _common = state.Scan(@"^(\s*)(common)\b", Color.Whitespace, Color.Keyword);
            if (_common)
            {
               var _commonBlock = getBlock(state);
               if (_commonBlock)
               {
                  commonBlock = _commonBlock;
               }
            }

            Maybe<IRangeItem> _ordinal = nil;

            while (state.More)
            {
               var _endBlock = state.EndBlock();
               if (_endBlock)
               {
                  break;
               }
               else if (_endBlock.Exception is (true, var exception))
               {
                  return exception;
               }

               var matchingParser = new EnumMemberParser(className, commonBlock, _ordinal);
               var _result = matchingParser.Scan(state);
               if (_result)
               {
                  if (matchingParser.Matching is (true, var matching))
                  {
                     var hasParameters = matchingParser.HasParameters;
                     var classBuilder = matching.ClassBuilder;
                     var name = classBuilder.UserClass.Name;
                     var parameters = classBuilder.Parameters;
                     var truncatedName = name.Substitute("^ -['$']+ '$' /(.+)$", "$1");
                     var selector = hasParameters ? parameters.Selector(truncatedName) : (Selector)truncatedName.get();
                     var constructorSelector = parameters.Selector(name);

                     _ordinal = matchingParser.Ordinal;

                     userClass.RegisterMember(constructorSelector, selector, _ordinal.Map(r => (IObject)r));
                  }
                  else
                  {
                     return fail("Class not provided for matching");
                  }
               }
               else
               {
                  return _result.Exception;
               }
            }
         }
         else
         {
            return _beginBlock.Exception;
         }
      }
      else
      {
         return _registered.Exception;
      }

      return unit;
   }
}