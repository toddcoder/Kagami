using System.Collections.Generic;
using Kagami.Library.Classes;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class TypeConstraintParser : SymbolParser
	{
		public TypeConstraintParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => "^ /(|s|) /'<' (> ['A-Z'])";

		public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			state.Colorize(tokens, Color.Whitespace, Color.Class);

			List<BaseClass> list = [];
			while (state.More)
         {
            var _name = state.Scan($"^ /(/s*) /({REGEX_CLASS})", Color.Whitespace, Color.Class);
            if (_name is (true, var name))
				{
					name = name.TrimStart();
					if (Module.Global.Class(name) is (true, var baseClass))
					{
						list.Add(baseClass);
					}
					else if (Module.Global.Forwarded(name))
					{
						list.Add(new ForwardedClass(name));
					}
					else
               {
                  return classNotFound(name);
               }
				}
				else if (_name.Exception is (true, var exception))
				{
					return exception;
				}
				else if (state.Scan("^ /'>'", Color.Class).If(out _, out anyException))
				{
					builder.Add(new TypeConstraintSymbol(list));
					return Unit.Matched();
				}
				else if (anyException.If(out exception))
				{
					return failedMatch<Unit>(exception);
				}
				else
				{
					return "Open type constraint".FailedMatch<Unit>();
				}
         }

			return Unit.Matched();
		}
	}
}