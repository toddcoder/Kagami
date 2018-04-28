using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements
{
   public class For : Statement
   {
      string fieldName;
      Expression source;
      Block block;

      public For(string fieldName, Expression source, Block block)
      {
         this.fieldName = fieldName;
         this.source = source;
         this.block = block;
      }

      public override void Generate(OperationsBuilder builder)
      {
         var topLabel = newLabel("top");
         var endLabel = newLabel("end");

         builder.PushFrame();
         var iteratorName = newLabel("iterator");
         builder.NewField(iteratorName, false, true);
         source.Generate(builder);
         builder.Peek(Index);
         builder.GetIterator(false);
         builder.AssignField(iteratorName, false);

         builder.Label(topLabel);
         builder.PushFrame();
         builder.NewField(fieldName, true, true);
         builder.GetField(iteratorName);
         builder.SendMessage("next", 0);
         builder.GoToIfNil(endLabel);

         builder.AssignField(fieldName, false);
         builder.GetField(fieldName);

         block.Generate(builder);
         builder.PopFrame();
         builder.GoTo(topLabel);

         builder.Label(endLabel);
         builder.PopFrame();
         builder.PopFrame();
      }

      public override string ToString() => $"for {fieldName} <- {source} {block}";
   }
}