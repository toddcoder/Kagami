using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class PrintStatement(PrintStatementType type, Expression expression) : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      switch (type)
      {
         case PrintStatementType.Print:
            expression.Generate(builder);
            builder.Print();
            break;
         case PrintStatementType.Println:
         {
            expression.Generate(builder);
            builder.PrintLine();
            break;
         }
         case PrintStatementType.Put:
         {
            expression.Generate(builder);
            builder.Put();
            break;
         }
      }
   }

   public override string ToString() => $"{type} {expression}";
}