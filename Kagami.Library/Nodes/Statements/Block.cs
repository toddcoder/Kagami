using System.Collections;
using System.Collections.Generic;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Expressions;
using Standard.Types.Enumerables;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Nodes.Statements
{
   public class Block : Statement, IEnumerable<Statement>
   {
      public static Block Getter(string fieldName)
      {
         var expressionBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
         expressionBuilder.Add(new FieldSymbol(fieldName));
         if (expressionBuilder.ToExpression().If(out var expression, out var exception))
            return new Block(new Return(expression));
         else
            throw exception;
      }

      public static Block Setter(string fieldName, string parameterName)
      {
         var expressionBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
         expressionBuilder.Add(new FieldSymbol(parameterName));
         if (expressionBuilder.ToExpression().If(out var expression, out var exception))
         {
            var assignToField = new AssignToField(fieldName, none<Operation>(), expression);
            return new Block(assignToField);
         }
         else
            throw exception;
      }

      List<Statement> statements;

      public Block(List<Statement> statements) => this.statements = statements;

      public Block(Statement statement) => statements = new List<Statement> { statement };

      public Block() => statements = new List<Statement>();

      public bool Yielding { get; set; }

      public override void Generate(OperationsBuilder builder)
      {
         foreach (var statement in statements)
            statement.Generate(builder);

         if (Yielding)
         {
            builder.PushNil();
            builder.Return(true);
         }
      }

      public IEnumerator<Statement> GetEnumerator() => statements.GetEnumerator();

      public override string ToString() => statements.Listify("\r\n");

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
   }
}