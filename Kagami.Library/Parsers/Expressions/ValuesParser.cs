﻿using System.Collections.Generic;

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
            yield return new MultiParameterLambdaParser(builder);
            yield return new SubexpressionParser2(builder);
            yield return new ArrayParser(builder);
            yield return new ListParser(builder);
            yield return new AnyParser(builder);
            yield return new FloatParser(builder);
            yield return new HexadecimalParser(builder);
            yield return new OctalParser(builder);
            yield return new BinaryParser(builder);
            yield return new ByteParser(builder);
            yield return new IntParser(builder);
            yield return new BooleanParser(builder);
            yield return new AlternateStringParser(builder);
            yield return new InterpolatedStringParser(builder);
            yield return new StringParser(builder);
            yield return new CharParser(builder);
            yield return new PeekParser(builder);

            if (!builder.Flags[ExpressionFlags.Subset])
            {
               if (!builder.Flags[ExpressionFlags.OmitColon])
                  yield return new NameValueParser(builder);

               yield return new BindingParser(builder);

               yield return new InvokeParser(builder);
            }

            yield return new FormatParser(builder);

            if (builder.Flags[ExpressionFlags.Comparisand])
               yield return new InternalListParser(builder);
            else
               yield return new SuperParser(builder);

            yield return new LazyParser(builder);
            yield return new KeywordValueParser(builder);
            yield return new ClassReferenceParser(builder);
            yield return new SymbolObjectParser(builder);
            yield return new WhateverParser(builder);
            yield return new FieldParser(builder);
         }
      }
   }
}