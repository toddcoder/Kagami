using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Runtime;
using Core.Booleans;
using Core.Strings;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Nodes.Statements
{
	public class Function : Statement
	{
		public static Function Getter(string fieldName)
		{
			return new Function($"__${fieldName}", new Parameters(0), Block.Getter(fieldName), false, false, "");
		}

		public static Function Setter(string fieldName)
		{
			var parameters = new Parameters(1);
			return new Function($"__${fieldName}=", parameters, Block.Setter(fieldName, parameters[0].Name), false, false, "");
		}

		protected Selector selector;
		protected Parameters parameters;
		protected Block block;
		protected bool yielding;
		protected bool overriding;
		protected string className;
		protected Lambda lambda;

		public Function(string functionName, Parameters parameters, Block block, bool yielding, bool overriding, string className)
		{
			selector = parameters.Selector(functionName);
			this.parameters = parameters;
			this.block = block;
			this.yielding = yielding;
			block.Yielding = this.yielding;
			this.overriding = overriding;
			this.className = className;
		}

		public void Deconstruct(out Selector selector, out Parameters parameters, out Block block, out bool yielding,
			out IInvokable invokable, out bool overriding)
		{
			selector = this.selector;
			parameters = this.parameters;
			block = this.block;
			yielding = this.yielding;
			invokable = GetInvokable();
			overriding = this.overriding;
		}

		public Selector Selector => selector;

		public Parameters Parameters => parameters;

		public Block Block => block;

		public string ClassName
		{
			get => className;
			set => className = value;
		}

		public bool Yielding => yielding;

		public bool Overriding => overriding;

		public bool Trait { get; set; }

		public IInvokable GetInvokable()
		{
			if (yielding)
			{
				return new YieldingInvokable(selector, parameters, ToString());
			}
			else
			{
				return new FunctionInvokable(selector, parameters, ToString());
			}
		}

		public override void Generate(OperationsBuilder builder)
		{
			var invokable = GetInvokable();
			lambda = new Lambda(invokable);
			if (builder.RegisterInvokable(invokable, block, overriding).If(out _, out var exception))
			{
				if (parameters[parameters.Length - 1].Variadic)
				{
					var lambdaName = selector.Name;
					lambdaName = className.IsNotEmpty() ? $"{className}.{lambdaName}" : lambdaName;
					if (!overriding)
					{
						builder.NewField(lambdaName, false, true);
					}

					builder.PushObject(lambda);
					builder.Peek(Index);
					builder.AssignField(lambdaName, overriding);
				}
				else
				{
					string fullFunctionName = selector;
					fullFunctionName = className.IsNotEmpty() ? $"{className}.{fullFunctionName}" : fullFunctionName;
					if (!overriding)
					{
						builder.NewSelector(selector, false, true);
					}

					builder.PushObject(lambda);
					builder.Peek(Index);
					builder.AssignSelector(selector, overriding);
				}
			}
			else
			{
				throw exception;
			}

			if (className.IsNotEmpty())
			{
				if (Trait)
				{
					if (Module.Global.Trait(className).If(out var trait))
					{
						if (trait.RegisterInvokable(selector, invokable).IfNot(out _, out exception))
						{
							throw exception;
						}
					}
					else
					{
						throw traitNotFound(className);
					}
				}
				else
				{
					if (Module.Global.Class(className).If(out var cls))
					{
						cls.RegisterMessage(selector, (obj, msg) => BaseClass.Invoke(obj, msg.Arguments, lambda));
					}
					else
					{
						throw classNotFound(className);
					}
				}
			}
		}

		public override string ToString()
		{
			return $"{overriding.Extend("override ")}{yielding.Extend("co")}func {selector.Image} ...";
		}
	}
}