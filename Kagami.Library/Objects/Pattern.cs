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

		public string Image => $"pattern {name}({lambda.Invokable.Parameters.Select(p => p.Name).Stringify()})";

		public int Hash => Image.GetHashCode();

		public bool IsEqualTo(IObject obj)
		{
			if (obj is Pattern pattern)
			{
				return name == pattern.name && lambda.IsEqualTo(pattern.lambda);
			}
			else
			{
				return false;
			}
		}

		protected bool returnValues(IObject result, Hash<string, IObject> bindings)
		{
			switch (result)
			{
				case Boolean boolean when arguments.Length == 0:
					return boolean.Value;
				case Boolean boolean when arguments.Length == 1:
					return match(boolean, arguments[0], bindings);
				case Some some when arguments.Length == 1:
					return match(some.Value, arguments[0], bindings);
				default:
					if (result is Some tupleSome && tupleSome.Value is Tuple tuple && tuple.Length.Value == arguments.Length)
					{
						return tuple.Value.Zip(arguments, (l, r) => match(l, r, bindings)).All(b => b);
					}

					break;
			}

			return false;
		}

		public virtual bool Match(IObject comparisand, Hash<string, IObject> bindings)
		{
			lambda.CopyFields(fields);
			var result = lambda.Invoke(comparisand);

			return returnValues(result, bindings);
		}

		public void RegisterArguments(Arguments arguments)
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

		public Pattern Copy() => new Pattern(name, lambda, parameters);
	}
}