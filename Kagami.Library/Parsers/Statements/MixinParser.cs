using Core.Collections;
using Core.Monads;
using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
    public class MixinParser : StatementParser
    {
        public override string Pattern => $"^ /'mixin' /(|s+|) /({REGEX_CLASS})";

        public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
        {
            var className = tokens[3].Text;
            state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class);

            state.SkipEndOfLine();
            state.Advance();

            state.SkipEndOfLine();
            state.Regress();

            if (getBlock(state).Out(out var block, out var originalBlock))
            {
                var builder = new ClassBuilder(className, Parameters.Empty, "", new Expression[0], false, block,
                    new Hash<string, TraitClass>());
                if (builder.Register().Out(out _, out var originalBuilder))
                {
                    var cls = new Class(builder);
                    state.AddStatement(cls);

                    var classItemsParser = new ClassItemsParser(builder);
                    while (state.More)
                    {
                        if (classItemsParser.Scan(state).If(out _, out var anyException)) { }
                        else if (anyException.If(out var exception))
                        {
                            return failedMatch<Unit>(exception);
                        }
                        else
                        {
                            break;
                        }
                    }

                    return Unit.Matched();
                }
                else
                {
                    return originalBuilder.Unmatched<Unit>();
                }
            }
            else
            {
                return originalBlock.Unmatched<Unit>();
            }
        }
    }
}