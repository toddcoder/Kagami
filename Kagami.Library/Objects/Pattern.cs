using System.Linq;
using Core.Collections;
using Core.Enumerables;
using Kagami.Library.Invokables;
using Kagami.Library.Runtime;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
	public class Pattern : IObject
	{
		protected string name;
		protected Lambda lambda;
		protected Fields fields;
		protected Parameters parameters;
		protected IObject[] arguments;

		public Pattern(string name, Lambda lambda, Parameters parameters)
		{
			this.name = name;
			this.lambda = lambda;
			this.parameters = parameters;

			fields = new Fields();
			foreach (var parameter in parameters)
			{
				fields.New(parameter.Name, true);
			}

			arguments = new IObject[0];
		}

		public string ClassName => "Pattern";

		public string AsString => name;

		public virtual string Image => $"pattern {name}({lambda.Invokable.Parameters.Select(p => p.Name).Stringify()})";

		public int Hash => Image.GetHashCode();

		public virtual bool IsEqualTo(IObject obj)
		{
			return obj is Pattern pattern && name == pattern.name && lambda.IsEqualTo(pattern.lambda);
		}

		public IObject Value { get; set; } = Boolean.False;

		protected bool returnValues(IObject result, Hash<string, IObject> bindings)
		{
			switch (result)
			{
				case Boolean boolean when arguments.Length == 0:
					Value = boolean;
					return boolean.Value;
				case Boolean boolean when arguments.Length == 1:
					Value = boolean;
					return match(boolean, arguments[0], bindings);
				case Some some when arguments.Length == 1:
					Value = some.Value;
					return match(some.Value, arguments[0], bindings);
				case SubPattern subPattern:
					Value = subPattern;
					return true;
				default:
					if (result is Some tupleSome && tupleSome.Value is Tuple tuple && tuple.Length.Value == arguments.Length)
					{
						Value = tuple;
						return tuple.Value.Zip(arguments, (l, r) => match(l, r, bindings)).All(b => b);
					}

					break;
			}

			Value = Boolean.False;
			return false;
		}

		public virtual bool Match(IObject comparisand, Hash<string, IObject> bindings)
		{
			lambda.CopyFields(fields);
			var result = lambda.Invoke(comparisand);

			return returnValues(result, bindings);
		}

		public virtual void RegisterArguments(Arguments arguments)
		{
			var fieldValues = arguments.Take(fields.Length).ToArray();
			var parameterNames = parameters.Select(p => p.Name).ToArray();
			var index = 0;
			foreach (var parameterName in parameterNames)
			{
				fields.Assign(parameterName, fieldValues[index++]);
			}

			this.arguments = arguments.Skip(fields.Length).ToArray();
		}

		public bool IsTrue => true;

		public virtual Pattern Copy() => new Pattern(name, lambda, parameters);

		public string Name => name;
	}
}