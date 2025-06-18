using Kagami.Library.Operations;
using Core.Enumerables;
using Core.Strings;

namespace Kagami.Library.Nodes.Symbols;

public class InterpolatedStringSymbol : Symbol
{
   protected string prefix;
   protected Expression[] expressions;
   protected string[] formats;
   protected string[] suffixes;
   protected bool isFailure;

   public InterpolatedStringSymbol(string prefix, Expression[] expressions, string[] formats, string[] suffixes, bool isFailure)
   {
      this.prefix = prefix;
      this.expressions = expressions;
      this.formats = formats;
      this.suffixes = suffixes;
      this.isFailure = isFailure;
   }

   public override void Generate(OperationsBuilder builder)
   {
      builder.PushString(prefix);

      var length = Math.Min(expressions.Length, suffixes.Length);
      for (var i = 0; i < length; i++)
      {
         expressions[i].Generate(builder);
         if (formats[i].IsEmpty())
         {
            builder.String();
         }
         else
         {
            builder.PushString(formats[i]);
            builder.SendMessage("format(_)", 1);
         }

         builder.Concatenate();
         builder.PushString(suffixes[i]);
         builder.Concatenate();
      }

      if (isFailure)
      {
         builder.Failure();
      }
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString()
   {
      return (StringStream)"$\"" / prefix / expressions.Zip(suffixes, (e, s) => $"({e}){s}").ToString("");
   }
}