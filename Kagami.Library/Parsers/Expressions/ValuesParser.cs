using System.Collections.Generic;

namespace Kagami.Library.Parsers.Expressions
{
   public class ValuesParser : MultiParser
   {
      ExpressionBuilder builder;

      public ValuesParser(ExpressionBuilder builder) => this.builder = builder;

      public override IEnumerable<Parser> Parsers
      {
         get
         {
            yield return new EmptyValueParser(builder);
            yield return new ZeroParameterLambdaParser(builder);
            yield return new OneParameterLambdaParser(builder);
            yield return new MatchLambdaParser(builder);
            yield return new MultiParameterLambdaParser(builder);
            yield return new TypeConstraintParser(builder);
            yield return new SubexpressionParser(builder);
            yield return new ArrayParser(builder);
            yield return new SelectorParser(builder);
            yield return new DictionaryParser(builder);
            yield return new CycleParser(builder);
            yield return new ListParser(builder);
            yield return new SetParser(builder);
            yield return new AnyParser(builder);
            yield return new FloatParser(builder);
            yield return new HexExpParser(builder);
            yield return new HexadecimalParser(builder);
            yield return new OctalParser(builder);
            yield return new BinaryParser(builder);
            yield return new ByteParser(builder);
            yield return new IntParser(builder);
            yield return new BooleanParser(builder);
            yield return new AlternateStringParser(builder);
            yield return new InterpolatedStringParser(builder);
            yield return new StringArrayParser(builder);
            yield return new DateParser(builder);
            yield return new MultilineStringParser(builder);
            yield return new StringParser(builder);
            yield return new CharParser(builder);
            yield return new ForExpressionParser(builder);
            yield return new ImplicitMessageParser(builder);
            yield return new ImplicitSymbolParser(builder);
            yield return new ImplicitExpressionParser(builder);
	         yield return new MessageParser(builder);

            if (!builder.Flags[ExpressionFlags.Subset])
            {
               yield return new BindingParser(builder);
	            yield return new InitializeParser(builder);
               yield return new InvokeParser(builder);
            }

            if (builder.Flags[ExpressionFlags.OmitColon])
            {
               yield return new NameValueParser(builder);
            }

            yield return new FormatParser(builder);

            if (builder.Flags[ExpressionFlags.Comparisand])
            {
	            yield return new InternalListParser(builder);
	            yield return new PlaceholderParser(builder);
            }
            else
            {
               yield return new ComparisandParser(builder);
               yield return new SuperParser(builder);
            }

            yield return new LazyParser(builder);
            yield return new KeywordValueParser(builder);
            yield return new ClassReferenceParser(builder);
            yield return new SymbolObjectParser(builder);
            yield return new WhateverParser(builder);
            yield return new RegexParser(builder);
            yield return new RefParser(builder);
	         yield return new DollarFieldParser(builder);
	         yield return new DoParser(builder);
	         yield return new TryBlockParser(builder);
	         yield return new TryParser(builder);
	         yield return new ThrowParser(builder);
	         yield return new AssertParser(builder);
	         yield return new SeqParser(builder);
	         yield return new ConversionParser(builder);
            yield return new FieldParser(builder);
         }
      }
   }
}