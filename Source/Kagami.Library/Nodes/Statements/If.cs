using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.Nodes.NodeFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Nodes.Statements
{
   public class If : Statement
   {
      protected Expression expression;
      protected Block block;
      protected IMaybe<If> _elseIf;
      protected IMaybe<Block> _elseBlock;
      protected string fieldName;
      protected bool mutable;
      protected bool assignment;
      protected bool top;

      public If(Expression expression, Block block, IMaybe<If> elseIf, IMaybe<Block> elseBlock, string fieldName, bool mutable, bool assignment,
         bool top)
      {
         this.expression = expression;
         this.block = block;
         _elseIf = elseIf;
         _elseBlock = elseBlock;
         this.fieldName = fieldName;
         this.mutable = mutable;
         this.assignment = assignment;
         this.top = top;
      }

      public If(Expression expression, Block block)
      {
         this.expression = expression;
         this.block = block;
         _elseIf = none<If>();
         _elseBlock = none<Block>();
         fieldName = "";
         mutable = false;
         assignment = false;
         top = false;
      }

      public void AddReturnIf()
      {
         block.AddReturnIf(new UnitSymbol());
         if (_elseBlock.If(out var elseBlock))
         {
            elseBlock.AddReturnIf(new UnitSymbol());
         }

         if (_elseIf.If(out var elseIf))
         {
            elseIf.AddReturnIf();
         }
      }

      public Maybe<If> ElseIf
      {
         get => _elseIf;
         set => _elseIf = value;
      }

      public Maybe<Block> Else
      {
         get => _elseBlock;
         set => _elseBlock = value;
      }

      public override void Generate(OperationsBuilder builder)
      {
         if (assignment && top)
         {
            builder.NewField(fieldName, mutable, true);
         }

         var nextLabel = newLabel("next");
         builder.PushLabel(LabelType.If, "end");
         builder.PushFrame();

         expression.Generate(builder);
         builder.Peek(Index);
         builder.GoToIfFalse(nextLabel);

         builder.PushFrame();
         block.Generate(builder);
         if (assignment)
         {
            builder.AssignField(fieldName, false);
         }

         builder.PopFrame();
         builder.PopFrame();
         builder.GoTo(LabelType.If);

         builder.Label(nextLabel);
         builder.PopFrame();
         if (_elseIf.If(out var elseIf))
         {
            elseIf.Generate(builder);
         }
         else if (_elseBlock.If(out var elseBlock))
         {
            builder.PushFrame();
            elseBlock.Generate(builder);
            if (assignment)
            {
               builder.AssignField(fieldName, false);
            }

            builder.PopFrame();
         }

         builder.Label(LabelType.If);
         builder.PopLabel(LabelType.If);
      }

      public override string ToString()
      {
         return (StringStream)"if " / expression / block % _elseIf.Map(i => $"else {i}").DefaultTo(() => "") %
            _elseBlock.Map(b => b.ToString()).DefaultTo(() => "");
      }
   }
}