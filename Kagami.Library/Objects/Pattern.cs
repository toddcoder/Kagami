using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Enumerables;
using Kagami.Library.Invokables;
using Kagami.Library.Runtime;

namespace Kagami.Library.Objects
{
	public struct Pattern : IObject
	{
		string name;
		Lambda lambda;
		Fields fields;
		List<string> bindingNames;
		Parameters parameters;

		public Pattern(string name, Lambda lambda, Parameters parameters) : this()
		{
			this.name = name;
			this.lambda = lambda;
			this.parameters = parameters;

			fields = new Fields();
			foreach (var parameter in parameters)
			{
				fields.New(parameter.Name, true);
			}
			bindingNames = new List<string>();
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

		public bool Match(IObject comparisand, Hash<string, IObject> bindings)
		{
			lambda.CopyFields(fields);
			var result = lambda.Invoke(comparisand);

			switch (bindingNames.Count)
			{
				case 0:
					return result.IsTrue;
				case 1:
					if (result is Some some && bindingNames.Count == 1)
					{
						bindings[bindingNames[0]] = some.Value;
						return true;
					}
					else
					{
						return false;
					}

				default:
					if (result is Some tupleSome && tupleSome.Value is Tuple tuple && tuple.Length.Value == bindingNames.Count)
					{
						for (var i = 0; i < bindingNames.Count; i++)
						{
							var bindingName = bindingNames[i];
							bindings[bindingName] = tuple[i];
						}
						return true;
					}
					else
					{
						return false;
					}
			}
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

			var argumentValues = arguments.Skip(fields.Length).ToArray();
			foreach (var argumentValue in argumentValues)
			{
				if (argumentValue is Placeholder placeholder)
				{
					bindingNames.Add(placeholder.Name);
				}
			}
		}

		public bool IsTrue => true;

		public Pattern Copy() => new Pattern(name, lambda, parameters);
	}
}