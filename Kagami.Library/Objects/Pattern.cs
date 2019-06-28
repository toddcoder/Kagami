using System.Linq;
using Core.Collections;
using Core.Enumerables;
using Kagami.Library.Runtime;

namespace Kagami.Library.Objects
{
	public struct Pattern : IObject
	{
		string name;
		Lambda lambda;
		Hash<string, string> placeholders;

		public Pattern(string name, Lambda lambda) : this()
		{
			this.name = name;
			this.lambda = lambda;
			placeholders = new Hash<string, string>();
			foreach (var parameter in lambda.Invokable.Parameters.Skip(1))
			{
				placeholders[parameter.Name] = "";
			}
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
			if (lambda.Invoke(comparisand).IsTrue)
			{
/*				foreach (var (fieldName, field) in fields)
				{
					if (placeholders.ContainsKey(fieldName))
					{
						bindings[placeholders[fieldName]] = field.Value;
					}
				}*/

				return true;
			}
			else
			{
				return false;
			}
		}

		public void RegisterArguments(Arguments arguments)
		{
			var pairs = lambda.Invokable.Parameters.Skip(1).Zip(arguments.Value, (p, a) => (p.Name, a));
			foreach (var (parameterName, argument)in pairs)
			{
				registerArgument(parameterName, argument);
			}
		}

		void registerArgument(string parameterName, IObject argument)
		{
			switch (argument)
			{
				case Placeholder placeholder:
					placeholders[parameterName] = placeholder.Name;
					break;
			}
		}

		public bool IsTrue => true;
	}
}