using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using System.Text.RegularExpressions;
using Core.Matching;
using Kagami.Library.Classes;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Class = Kagami.Library.Nodes.Statements.Class;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class EnumParser2 : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(enum)(\s+)({REGEX_CLASS})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var className = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class);

      Module.Global.Value.ForwardReference(className);

      var _beginBlock = state.BeginBlock();
      if (!_beginBlock)
      {
         return _beginBlock.Exception;
      }

      List<EnumMemberData> enumMembers = [];
      Optional<Unit> _endBlock = nil;

      while (state.More)
      {
         _endBlock = state.EndBlock();
         if (_endBlock)
         {
            break;
         }
         else if (_endBlock.Exception is (true, var exception))
         {
            return exception;
         }

         var enumMemberParser = new EnumMemberParser2(className);
         var _enumMember = enumMemberParser.Scan(state);
         if (_enumMember)
         {
            if (enumMemberParser.EnumMemberData is (true, var enumMemberData))
            {
               enumMembers.Add(enumMemberData);
            }
         }
         else if (_enumMember.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            break;
         }
      }

      Maybe<Block> _block;
      if (_endBlock)
      {
         _block = nil;
      }
      else
      {
         _block = getPartialBlock(state).Maybe();
      }

      var enumClassBuilder = new EnumClassBuilder(className);
      var _enumRegistered = enumClassBuilder.Register();
      if (!_enumRegistered)
      {
         if (_enumRegistered.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            return fail("Failed to register enum class");
         }
      }

      var userClass = (EnumClass)enumClassBuilder.UserClass;

      foreach (var enumMemberData in enumMembers)
      {
         var fullBlock = new Block();
         if (_block is (true, var block))
         {
            fullBlock = block;
            if (enumMemberData.Block is (true, var memberBlock))
            {
               fullBlock = Block.Merge(fullBlock, memberBlock);
            }
         }
         else if (enumMemberData.Block is (true, var memberBlock))
         {
            fullBlock = memberBlock;
         }

         var (name, parameters, _ordinal, _) = enumMemberData;

         var truncatedName = name.Substitute("^ -['$']+ '$' /(.+)$", "$1");
         var selector = parameters.Length > 0 ? parameters.Selector(truncatedName) : (Selector)truncatedName.get();
         var constructorSelector = parameters.Selector(name);

         userClass.RegisterMember(constructorSelector, selector, _ordinal);

         var enumMemberClassBuilder = new EnumMemberClassBuilder(name, parameters, fullBlock)
         {
            Selector = enumMemberData.Parameters.Selector(enumMemberData.Name),
            Ordinal = enumMemberData.Ordinal
         };
         var _registered = enumMemberClassBuilder.Register();
         if (_registered)
         {
            var cls = new Class(enumMemberClassBuilder);
            state.AddStatement(cls);
         }
         else
         {
            return _registered.Exception;
         }
      }

      return unit;
   }
}