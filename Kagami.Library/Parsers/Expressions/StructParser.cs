using System.Text.RegularExpressions;
using Core.Enumerables;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Statements;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class StructParser : SymbolParser
{
   public StructParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(struct)(\{)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.OpenParenthesis);

      var _taggedExpressions = getTaggedExpressions(state, REGEX_BLOCK_END);
      if (_taggedExpressions is (true, var taggedExpressions))
      {
         var className = taggedExpressions.Select(te => te.Tag.ToUpper1()).ToString("$");
         Module.Global.Value.ForwardReference(className);
         List<Statement> statements = [];
         foreach (var (tag, expression) in taggedExpressions)
         {
            var assignToNewField = new AssignToNewField(true, tag, false, expression);
            statements.Add(assignToNewField);
         }

         var block = new Block(statements);
         var classBuilder = new ClassBuilder(className, Parameters.Empty, "", [], false, block);
         var _registered = classBuilder.Register();
         if (_registered)
         {
            var cls = new Class(classBuilder);
            state.AddStatement(cls);
            builder.Add(new InvokeSymbol(className, [], nil, false));
            return unit;
         }
         else
         {
            return _registered.Exception;
         }
      }
      else
      {
         return _taggedExpressions.Exception;
      }
   }
}