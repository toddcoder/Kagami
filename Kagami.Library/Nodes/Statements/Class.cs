using Kagami.Library.Operations;
using Kagami.Library.Parsers.Statements;

namespace Kagami.Library.Nodes.Statements;

public class Class : Statement
{
   protected ClassBuilder classBuilder;

   public Class(ClassBuilder classBuilder)
   {
      this.classBuilder = classBuilder;
   }

   public ClassBuilder ClassBuilder => classBuilder;

   public override void Generate(OperationsBuilder builder) => classBuilder.Generate(builder);

   public override string ToString() => classBuilder.ToString();

   public bool IsFixed => classBuilder.IsFixed;
}