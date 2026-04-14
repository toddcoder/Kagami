using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.CommonFunctions;
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
      var enumName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class);

      Module.Global.Value.ForwardReference(enumName);

      var _beginBlock = state.Scan(@"^(\s*)(\{)", Color.Whitespace, Color.Block);
      if (!_beginBlock)
      {
         return _beginBlock.Exception;
      }

      List<EnumMemberData> enumMembers = [];
      var value = Int.Zero;
      var scanning = true;

      while (state.More && scanning)
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

         var enumMemberParser = new EnumMemberParser(value);
         var _scanned = enumMemberParser.Scan(state);
         if (_scanned)
         {
            if (enumMemberParser.EnumMemberData is (true, var enumMemberData))
            {
               enumMembers.Add(enumMemberData);
               value = (IObject)((IRangeItem)enumMemberData.Value).Successor;
            }
         }
         else
         {
            return _scanned.Exception;
         }
      }

      var builder = new ClassBuilder(enumName, Parameters.Empty, "", [], false, new Block());
      var _register = builder.Register();
      if (_register)
      {
         var cls = new Class(builder);
         state.AddStatement(cls);

         var block = new Block();
         foreach (var enumMemberData in enumMembers)
         {
            var expression = new Expression(new ObjectSymbol(enumMemberData.Value));
            var assignToNewField = new AssignToNewField(false, enumMemberData.Name, expression, false, false);
            block.Add(assignToNewField);
         }

         var enumMetaName = metaName(enumName);
         var metaBuilder = new ClassBuilder(enumMetaName, Parameters.Empty, "", [], false, block);
         _register = metaBuilder.Register();
         if (_register)
         {
            var metaClass = new MetaClass(enumName, metaBuilder);
            state.AddStatement(metaClass);

            return unit;
         }
         else
         {
            return _register.Exception;
         }
      }
      else
      {
         return _register.Exception;
      }
   }
}