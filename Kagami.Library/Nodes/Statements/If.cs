using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.Nodes.NodeFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Nodes.Statements;

public class If : Statement
{
   protected Expression expression;
   protected Block block;
   protected Maybe<If> _elseIf;
   protected Maybe<Block> _elseBlock;
   protected string fieldName;
   protected bool mutable;
   protected bool assignment;
   protected bool top;
   protected bool retainExpressionFields;

   public If(Expression expression, Block block, Maybe<If> _elseIf, Maybe<Block> _elseBlock, string fieldName, bool mutable, bool assignment,
      bool top, bool retainExpressionFields = false)
   {
      this.expression = expression;
      this.block = block;
      this._elseIf = _elseIf;
      this._elseBlock = _elseBlock;
      this.fieldName = fieldName;
      this.mutable = mutable;
      this.assignment = assignment;
      this.top = top;
      this.retainExpressionFields = retainExpressionFields;
   }

   public If(Expression expression, Block block)
   {
      this.expression = expression;
      this.block = block;
      _elseIf = nil;
      _elseBlock = nil;
      fieldName = "";
      mutable = false;
      assignment = false;
      top = false;
   }

   public void AddReturnIf()
   {
      block.AddReturnIf(new UnitSymbol());
      if (_elseBlock is (true, var elseBlock))
      {
         elseBlock.AddReturnIf(new UnitSymbol());
      }

      if (_elseIf is (true, var elseIf))
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
      if (!retainExpressionFields)
      {
         builder.PushFrame();
      }

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
      if (!retainExpressionFields)
      {
         builder.PopFrame();
      }

      builder.GoTo(LabelType.If);

      builder.Label(nextLabel);
      if (!retainExpressionFields)
      {
         builder.PopFrame();
      }

      if (_elseIf is (true, var elseIf))
      {
         elseIf.Generate(builder);
      }
      else if (_elseBlock is (true, var elseBlock))
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
      return (StringStream)"if " / expression / block % (_elseIf.Map(i => $"else {i}") | (() => "")) %
         (_elseBlock.Map(b => b.ToString()) | (() => ""));
   }
}