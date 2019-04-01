using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Core.Monads;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements
{
	public class AssignFromLoop : Statement
	{
		bool isNew;
		bool mutable;
		string fieldName;
		IMaybe<TypeConstraint> typeConstraint;
		Block block;
		Expression condition;
		Expression expression;

		public AssignFromLoop(bool isNew, bool mutable, string fieldName, IMaybe<TypeConstraint> typeConstraint, Block block, Expression condition, Expression expression)
		{
			this.isNew = isNew;
			this.mutable = mutable;
			this.fieldName = fieldName;
			this.typeConstraint = typeConstraint;
			this.block = block;
			this.condition = condition;
			this.expression = expression;
		}

		public override void Generate(OperationsBuilder builder)
		{
			var beginLabel = newLabel("begin");
			var exitLabel = newLabel("exit");
			var skipLabel = newLabel("skip");
			var untilLabel = newLabel("until");
			var endLabel = newLabel("end");

			if (isNew)
				builder.NewField(fieldName, mutable, true, typeConstraint);

			builder.Label(beginLabel);

			builder.PushFrame();
			builder.PushExitFrame(exitLabel);
			builder.PushSkipFrame(skipLabel);

			block.Generate(builder);

			builder.PopFrame();
			builder.Label(skipLabel);
			builder.PopFrame();

			condition.Generate(builder);
			builder.GoToIfFalse(untilLabel);
			expression.Generate(builder);

			builder.PopFrameWithValue();
			builder.AssignField(fieldName, true);

			builder.Label(exitLabel);
			builder.GoTo(endLabel);

			builder.Label(untilLabel);
			builder.PopFrame();
			builder.GoTo(beginLabel);

			builder.Label(endLabel);
			builder.NoOp();
      }
	}
}