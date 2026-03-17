namespace Kagami.Library.Parsers.Expressions;

public class ValuesParser : MultiParser
{
   protected ExpressionBuilder builder;

   public ValuesParser(ExpressionBuilder builder)
   {
      this.builder = builder;
   }

   public override IEnumerable<Parser> Parsers
   {
      get
      {
         yield return new ClassSendMessageAssignParser(builder);
         yield return new ClassSendMessageParser(builder);
         yield return new ForwardReductionParser(builder);
         yield return new EmptyMemoParser(builder);
         yield return new AltCharParser(builder);
         yield return new EmptyTypedCollectionParser(builder);
         yield return new WhateverLambdaParser(builder);
         yield return new SimpleLambdaParser(builder);
         yield return new ZeroParameterLambdaParser(builder);
         yield return new OneParameterLambdaParser(builder);
         yield return new MatchLambdaParser(builder);
         yield return new MultiParameterLambdaParser(builder);
         yield return new BlockValueParser(builder);
         yield return new BlockLambdaParser(builder);
         yield return new TypeConstraintParser(builder);
         yield return new ImplicitParameterLambdaParser(builder);
         yield return new SubexpressionParser(builder);
         yield return new EndOfExpressionParser(builder, this);
         yield return new ArrayParser(builder);
         yield return new SelectorParser(builder);
         yield return new DictionaryOrSetParser(builder);
         yield return new CycleParser(builder);
         yield return new ListParser(builder);
         yield return new PendingInvokeParser(builder);
         yield return new PendingSendMessageParser(builder);
         yield return new AnyParser(builder);
         yield return new FloatParser(builder);
         yield return new HexExpParser(builder);
         yield return new HexadecimalParser(builder);
         yield return new OctalParser(builder);
         yield return new BinaryParser(builder);
         yield return new ByteParser(builder);
         yield return new IntParser(builder);
         yield return new OtherwiseParser(builder);
         yield return new BooleanParser(builder);
         yield return new RawStringParser(builder);
         yield return new InterpolatedStringParser(builder);
         yield return new LazyStringParser(builder);
         yield return new StringArrayParser(builder);
         yield return new DateParser(builder);
         yield return new MultilineStringParser(builder);
         yield return new StringParser(builder);
         yield return new CharParser(builder);
         yield return new ForExpressionParser(builder);
         yield return new JunctionParser(builder);
         yield return new MessageParser(builder);

         if (!builder.Flags[ExpressionFlags.Subset])
         {
            yield return new InitializeParser(builder);

            if (!builder.Flags[ExpressionFlags.Comparisand])
            {
               yield return new DslInvokeParser(builder);
               yield return new InvokeParser(builder);
            }
         }

         yield return new NameValueParser(builder);

         yield return new FormatParser(builder);
         yield return new WhitespaceParser(builder);

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

         yield return new KeywordValueParser(builder);
         yield return new AliasedClassReferenceParser(builder);
         yield return new ClassReferenceParser(builder);
         yield return new SymbolObjectParser(builder);
         yield return new WhateverParser(builder);
         yield return new RegexParser(builder);
         yield return new StandardRegexParser(builder);
         yield return new RefParser(builder);
         yield return new ImplicitParameterParser(builder);
         yield return new DollarFieldParser(builder);
         yield return new DoParser(builder);
         yield return new ThrowParser(builder);
         yield return new RecordParser(builder);

         yield return new AssertParser(builder);
         yield return new ConversionParser(builder);
         yield return new IterParser(builder);
         yield return new NameOfParser(builder);
         yield return new LastValueParser(builder);
         yield return new TryParser(builder);

         yield return new FieldParser(builder);
      }
   }

   public bool IsEndOfExpression { get; set; }
}