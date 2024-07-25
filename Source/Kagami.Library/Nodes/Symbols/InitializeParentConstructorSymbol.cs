using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
	public class InitializeParentConstructorSymbol : InitializeSymbol
	{
		public InitializeParentConstructorSymbol(string className, (string, Expression)[] arguments) : base(className, arguments) { }

		public override void Generate(OperationsBuilder builder)
		{
			base.Generate(builder);
			builder.FieldsFromObject();
		}
	}
}