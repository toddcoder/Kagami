﻿using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Kagami.Library.Nodes.NodeFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Nodes.Statements
{
   public class If : Statement
   {
      Expression expression;
      Block block;
      IMaybe<If> elseIf;
      IMaybe<Block> elseBlock;
      string fieldName;
      bool mutable;
      bool assignment;
      bool top;

      public If(Expression expression, Block block, IMaybe<If> elseIf, IMaybe<Block> elseBlock, string fieldName,
         bool mutable, bool assignment, bool top)
      {
         this.expression = expression;
         this.block = block;
         this.elseIf = elseIf;
         this.elseBlock = elseBlock;
         this.fieldName = fieldName;
         this.mutable = mutable;
         this.assignment = assignment;
         this.top = top;
      }

      public If(Expression expression, Block block)
      {
         this.expression = expression;
         this.block = block;
         elseIf = none<If>();
         elseBlock = none<Block>();
         fieldName = "";
         mutable = false;
         assignment = false;
         top = false;
      }

      public IMaybe<If> ElseIf
      {
         get => elseIf;
         set => elseIf = value;
      }

      public IMaybe<Block> Else
      {
         get => elseBlock;
         set => elseBlock = value;
      }

      public override void Generate(OperationsBuilder builder)
      {
         if (assignment && top)
            builder.NewField(fieldName, mutable, true);

         var nextLabel = newLabel("next");
         builder.PushLabel(LabelType.If, "end");
         builder.PushFrame();

         expression.Generate(builder);
         builder.Peek(Index);
         builder.GoToIfFalse(nextLabel);

         builder.PushFrame();
         block.Generate(builder);
         if (assignment)
            builder.AssignField(fieldName, false);
         builder.PopFrame();
         builder.PopFrame();
         builder.GoTo(LabelType.If);

         builder.Label(nextLabel);
         builder.PopFrame();
         if (elseIf.If(out var ei))
            ei.Generate(builder);
         else if (elseBlock.If(out var eBlock))
         {
            builder.PushFrame();
            eBlock.Generate(builder);
            if (assignment)
               builder.AssignField(fieldName, false);
            builder.PopFrame();
         }

         builder.Label(LabelType.If);
         builder.PopLabel(LabelType.If);
      }

      public override string ToString()
      {
         return (StringStream)"if " / expression / block % elseIf.FlatMap(i => $"else {i}", () => "") %
            elseBlock.FlatMap(b => b.ToString(), () => "");
      }
   }
}