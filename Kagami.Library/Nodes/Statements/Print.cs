using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringStreamFunctions;

namespace Kagami.Library.Nodes.Statements
{
   public class Print : Statement
   {
      bool newLine;
      IMaybe<Expression> expression;

      public Print(bool newLine, Expression expression)
      {
         this.newLine = newLine;
         this.expression = expression.Some();
      }

      public Print()
      {
         newLine = true;
         expression = none<Expression>();
      }

      public override void Generate(OperationsBuilder builder)
      {
         if (expression.If(out var e))
         {
	         e.Generate(builder);
         }
         else
         {
	         builder.PushString("");
         }

         builder.Peek(Index);

         if (newLine)
         {
	         builder.PrintLine();
         }
         else
         {
	         builder.Print();
         }
      }

      public override string ToString()
      {
         return stream() / "print" / (newLine ? "ln " : " ") / expression.FlatMap(e => e.ToString(), () => "");
      }
   }
}