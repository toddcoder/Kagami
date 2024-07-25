using Kagami.Library.Operations;
using Kagami.Library.Parsers.Statements;

namespace Kagami.Library.Nodes.Statements
{
   public class MetaClass : Statement
   {
      protected string className;
      protected ClassBuilder classBuilder;

      public MetaClass(string className, ClassBuilder classBuilder)
      {
         this.className = className;
         this.classBuilder = classBuilder;
      }

      public override void Generate(OperationsBuilder builder)
      {
         classBuilder.Generate(builder, Index);
         builder.AssignMetaObject(className, classBuilder.UserClass.Name);
      }

      public override string ToString() => classBuilder.ToString();
   }
}