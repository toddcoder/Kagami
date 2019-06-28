using System.Linq;
using Core.Enumerables;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
	public class Pattern : Statement
	{
		string name;
		Parameters parameters;
		Block block;
		string image;

		public Pattern(string name, Parameters parameters, Block block)
		{
			this.name = name;
			this.parameters = parameters;
			this.block = block;
			image = $"{name}({parameters.Select(p => "_").Stringify(",")})";
        }

		public override void Generate(OperationsBuilder builder)
		{
			var invokable = new FunctionInvokable(name, parameters, name);
			if (builder.RegisterInvokable(invokable, block, true).If(out _, out var exception))
			{
				var lambda = new Lambda(invokable);
				var pattern = new Objects.Pattern(name, lambda);
				builder.NewField(name, false, true);
				builder.PushObject(pattern);
				builder.Peek(Index);
				builder.AssignField(name, true);
			}
			else
			{
				throw exception;
			}
		}

		public override string ToString() => image;
    }
}