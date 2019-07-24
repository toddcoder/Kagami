using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.CommonFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class ClassBuilder
	{
		string className;
		Parameters parameters;
		string parentClassName;
		Expression[] parentArguments;
		bool initialize;
		Block constructorBlock;
		Hash<string, (ConstructorInvokable, Block)> constructorInvokables;
		List<(IInvokable, Block, bool)> functions;
		UserClass userClass;
		IEnumerable<Mixin> mixins;

		public ClassBuilder(string className, Parameters parameters, string parentClassName, Expression[] parentArguments,
			bool initialize, Block constructorBlock, IEnumerable<Mixin> mixins)
		{
			this.className = className;
			this.parameters = parameters;
			this.parentClassName = parentClassName;
			this.parentArguments = parentArguments;
			this.initialize = initialize;
			this.constructorBlock = constructorBlock;
			constructorInvokables = new Hash<string, (ConstructorInvokable, Block)>();
			functions = new List<(IInvokable, Block, bool)>();
			this.mixins = mixins;
		}

		public IMatched<Unit> Register()
		{
			userClass = new UserClass(className, parentClassName);
			if (Module.Global.RegisterClass(userClass).IfNot(out var exception))
			{
				return failedMatch<Unit>(exception);
			}
			else
			{
				return Constructor(parameters, constructorBlock, true);
			}
		}

		public UserClass UserClass => userClass;

		public Statement[] Statements { get; set; } = new Statement[0];

		static bool isPrivate(string identifier) => identifier.IsMatch("^ '_' -(> '_$')");

		(string, Expression)[] getInitializeArguments()
		{
			return parentArguments.Select(e => e.Symbols[0]).Cast<NameValueSymbol>().Select(nv => nv.Tuple()).ToArray();
		}

		Block modifyBlock(Block originalBlock, bool standard)
		{
			userClass.RegisterParameters(parameters);

			var statements = new List<Statement>();

			if (parentClassName.IsNotEmpty())
			{
				if (Module.Global.Class(parentClassName).If(out var baseClass))
				{
					var parentClass = (UserClass)baseClass;
					if (standard)
					{
						userClass.InheritFrom(parentClass);
					}

					var symbol = initialize ? (Symbol)new InitializeParentConstructorSymbol(parentClassName, getInitializeArguments())
						: new InvokeParentConstructorSymbol(parentClassName, parentArguments, false);
					statements.Add(new ExpressionStatement(symbol, false));
				}
				else
				{
					throw classNotFound(parentClassName);
				}
			}

			foreach (var statement in originalBlock)
			{
				switch (statement)
				{
					case AssignToNewField assignToNewField:
					{
						var (mutable, fieldName, _) = assignToNewField;
						var function = Function.Getter(fieldName);
						statements.Add(function);
						var (functionName, _, block, _, invokable, _) = function;
						if (!isPrivate(fieldName) && !userClass.RegisterMethod(functionName, new Lambda(invokable), true))
						{
							throw needsOverride(functionName);
						}

						functions.Add((invokable, block, true));

						if (mutable)
						{
							function = Function.Setter(fieldName);
							statements.Add(function);
							(functionName, _, block, _, invokable, _) = function;
							if (!isPrivate(fieldName) && !userClass.RegisterMethod(functionName, new Lambda(invokable), true))
							{
								throw needsOverride(functionName);
							}

							functions.Add((invokable, block, true));
						}

						statements.Add(statement);
					}
						break;
					case AssignToNewField2 assignToNewField2:
					{
						var (comparisand, _) = assignToNewField2;
						if (comparisand.Symbols[0] is PlaceholderSymbol placeholder)
						{
							var fieldName = placeholder.Name;
							var (bindingType, name) = fromBindingName(fieldName);
							var function = Function.Getter(name);
							statements.Add(function);
							var (functionName, _, block, _, invokable, _) = function;
							if (!isPrivate(fieldName) && !userClass.RegisterMethod(functionName, new Lambda(invokable), true))
							{
								throw needsOverride(functionName);
							}

							functions.Add((invokable, block, true));

							if (bindingType == BindingType.Mutable)
							{
								function = Function.Setter(fieldName);
								statements.Add(function);
								(functionName, _, block, _, invokable, _) = function;
								if (!isPrivate(fieldName) && !userClass.RegisterMethod(functionName, new Lambda(invokable), true))
								{
									throw needsOverride(functionName);
								}

								functions.Add((invokable, block, true));
							}

							statements.Add(statement);
						}
					}
						break;
					case Function function when standard:
					{
						var (selector, _, block, _, invokable, overriding) = function;
						if (!isPrivate(selector))
						{
							if (userClass.RegisterMethod(selector, new Lambda(invokable), overriding))
							{
								functions.Add((invokable, block, overriding));
							}
							else
							{
								throw needsOverride(selector);
							}
						}

						statements.Add(statement);
					}
						break;
					case MatchFunction matchFunction when standard:
					{
						var (functionName, _, block, _, invokable, overriding) = matchFunction;
						if (!isPrivate(functionName))
						{
							if (userClass.RegisterMethod(functionName, new Lambda(invokable), overriding))
							{
								functions.Add((invokable, block, overriding));
							}
							else
							{
								throw needsOverride(functionName);
							}
						}

						statements.Add(statement);
					}
						break;
					default:
						statements.Add(statement);
						break;
				}
			}

			foreach (var mixin in mixins)
			{
				userClass.Include(mixin);
			}

			statements.Add(new ReturnNewObject(className, parameters));

			Statements = statements.ToArray();

			return new Block(statements);
		}

		public IMatched<Unit> Constructor(Parameters parameters, Block block, bool standard)
		{
			var invokable = new ConstructorInvokable(className, parameters);
			var fullFunctionName = parameters.Selector(className);
			if (constructorInvokables.ContainsKey(fullFunctionName))
			{
				return $"Constructor {fullFunctionName} already exists".FailedMatch<Unit>();
			}
			else
			{
				constructorInvokables[fullFunctionName] = (invokable, modifyBlock(block, standard));
				return Unit.Matched();
			}
		}

		public void Generate(OperationsBuilder builder, int index)
		{
			foreach (var item in constructorInvokables)
			{
				Selector selector = item.Key;
				var (invokable, block) = item.Value;
				if (builder.RegisterInvokable(invokable, block, true).IfNot(out var exception))
				{
					throw exception;
				}

				builder.NewSelector(selector, false, true);
				builder.PushObject(new Constructor(invokable));
				builder.Peek(index);
				builder.AssignSelector(selector, true);
			}

			foreach (var function in functions)
			{
				var (invokable, block, overriding) = function;
				if (builder.RegisterInvokable(invokable, block, overriding).IfNot(out var exception))
				{
					throw exception;
				}
			}
		}

		public override string ToString()
		{
			return $"class {className}({parameters}){parentClassName.Extend(" of ", $"({parentArguments.Stringify()})")}";
		}
	}
}