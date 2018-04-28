using Kagami.Library.Operations;
using Kagami.Library.Parsers.Statements;

namespace Kagami.Library.Nodes.Statements
{
   public class Class : Statement
   {
      ClassBuilder classBuilder;

      public Class(ClassBuilder classBuilder) => this.classBuilder = classBuilder;

      public override void Generate(OperationsBuilder builder) => classBuilder.Generate(builder, Index);

      public override string ToString() => classBuilder.ToString();
   }
}