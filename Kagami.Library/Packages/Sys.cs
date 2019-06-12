using System;
using System.Linq;
using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;
using Boolean = Kagami.Library.Objects.Boolean;
using String = Kagami.Library.Objects.String;
using Tuple = Kagami.Library.Objects.Tuple;

namespace Kagami.Library.Packages
{
	public class Sys : Package
	{
		public Sys()
		{
			fields.New("id", new RuntimeLambda(args => args[0], 1, "x -> x"));
		}

		public override string ClassName => "Sys";

		public IObject ID => fields["id"];

		public override void LoadTypes(Module module)
		{
			module.RegisterClass(new SysClass());
			module.RegisterClass(new RegexMatchClass());
			module.RegisterClass(new RegexGroupClass());
			module.RegisterClass(new RandomClass());
		}

		public String Println(Arguments arguments)
		{
			var value = arguments.Select(a => a.AsString).Stringify(" ");
			Machine.Current.Context.PrintLine(value);

			return value;
		}

		public String Print(Arguments arguments)
		{
			var value = arguments.Select(a => a.AsString).Stringify(" ");
			Machine.Current.Context.Print(value);

			return value;
		}

		public String Put(Arguments arguments)
		{
			var value = arguments.Select(a => a.AsString).Stringify(" ");

			foreach (var argument in arguments)
				Machine.Current.Context.Put(argument.AsString);

			return value;
		}

		public IObject Readln() => Machine.Current.Context.ReadLine().FlatMap(s => new Some(String.StringObject(s)), () => None.NoneValue);

		public IObject Peek(IObject obj)
		{
			Machine.Current.Context.PrintLine(obj.Image);
			return obj;
		}

		public IObject Peek(IObject prefix, IObject obj)
		{
			Machine.Current.Context.PrintLine($"{prefix.AsString}: {obj.Image}");
			return obj;
		}

		public IResult<IObject> Match(IObject x, IObject y)
		{
			if (y is Pattern pattern)
				return MatchToPattern(pattern, x);
			else
			{
				var bindings = new Hash<string, IObject>();
				if (x.Match(y, bindings))
				{
					Machine.Current.CurrentFrame.Fields.SetBindings(bindings);
					return Boolean.True.Success();
				}
				else
					return Boolean.False.Success();
			}
		}

		public IResult<IObject> MatchToPattern(Pattern pattern, IObject source)
		{
			try
			{
				var cases = pattern.Cases;
				foreach (var (comparisand, lambda) in cases)
				{
					var bindings = new Hash<string, IObject>();
					var frame = new Frame();
					Machine.Current.PushFrame(frame);
					if (source.Match(comparisand, bindings))
					{
						frame.Fields.SetBindings(bindings);
						var result = lambda.Invoke();
						frame.Pop();
						return result.Success();
					}

					frame.Pop();
				}

				return "No match".Failure<IObject>();
			}
			catch (Exception exception)
			{
				return failure<IObject>(exception);
			}
		}

		public Long Ticks() => new Long(DateTime.Now.Ticks);

		public IResult<IObject> NewParameterlessObject(string className, string fieldName)
		{
			try
			{
				var userObject = new UserObject(className, new Fields(), Parameters.Empty);
				if (Machine.Current.CurrentFrame.Fields.New(fieldName, userObject).IfNot(out var exception))
					return failure<IObject>(exception);
				else
					return userObject.Success<IObject>();
			}
			catch (Exception exception)
			{
				return failure<IObject>(exception);
			}
		}

		public IObject First(Tuple tuple) => tuple[0];

		public IObject Second(Tuple tuple) => tuple[1];

		public IResult<IObject> GetReference(string fieldName)
		{
			return Machine.Current.Find(fieldName, true).FlatMap(f => new Reference(f).Success<IObject>(),
				() => failure<IObject>(fieldNotFound(fieldName)), failure<IObject>);
		}

		public IObject Tuple(IObject value) => new Tuple(value);

		public IObject Tuple(IObject value1, IObject value2) => new Tuple(value1, value2);

		public RegexGroup RegexGroup(Arguments arguments)
		{
			var passed = new Hash<string, IObject>
			{
				["text"] = arguments[0],
				["index"] = arguments[1],
				["length"] = arguments[2]
			};

			return new RegexGroup(passed);
		}

		public RegexMatch RegexMatch(Arguments arguments)
		{
			var passed = new Hash<string, IObject>
			{
				["text"] = arguments[0],
				["index"] = arguments[1],
				["length"] = arguments[2],
				["groups"] = arguments[3]
			};

			return new RegexMatch(passed);
		}

		public XRandom Random() => new XRandom();

		public XRandom Random(int seed) => new XRandom(seed);

		public Complex Complex(IObject real, IObject imaginary)
		{
			if (real is INumeric rNumeric)
			{
				var doubleR = rNumeric.AsDouble();
				if (imaginary is INumeric iNumeric)
				{
					var doubleI = iNumeric.AsDouble();
					return new Complex(doubleR, doubleI);
				}
				else
					throw notNumeric(imaginary);
			}
			else
				throw notNumeric(real);
		}

		public Selector Selector(string source) => source;

		public Dictionary XFields()
		{
			return new Dictionary(Machine.Current.CurrentFrame.Fields.ToHash(t => String.StringObject(t.fieldName), t => t.field.Value));
		}

		public Date Date(double floating) => DateTime.FromOADate(floating);

		public Regex Regex(string pattern)
		{
			if (pattern.Matches("';' /(['IiMmGgTt']+) $").If(out var matcher))
			{
				var ignoreCase = false;
				var multiline = false;
				var global = false;
				var textOnly = true;
				foreach (var option in matcher.FirstGroup)
					switch (option)
					{
						case 'I':
						case 'i':
							ignoreCase = true;
							break;
						case 'M':
						case 'm':
							multiline = true;
							break;
						case 'G':
						case 'g':
							global = true;
							break;
						case 'T':
						case 't':
							textOnly = true;
							break;
					}
				pattern = pattern.Drop(-matcher.Length);
				return new Regex(pattern, ignoreCase, multiline, global, textOnly);
			}
			else
				return new Regex(pattern, false, false, false, false);
		}
	}
}