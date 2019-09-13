using System;
using System.Diagnostics;
using Core.Applications;
using Core.Computers;
using Core.Dates;
using Kagami.Library;
using Kagami.Library.Runtime;
using Core.Monads;
using static System.Console;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami
{
	internal class Program : CommandLineInterface, IContext
	{
		Putter putter;

		public Program() => putter = new Putter();

		static void Main()
		{
			using (var program = new Program())
			{
				program.Run("--", " ");
			}
		}

		[EntryPoint(EntryPointType.This)]
		public void EntryPoint()
		{
			if (Exec)
			{
				exec();
			}
		}

		void exec()
		{
			if (File.If(out var sourceFile))
			{
				if (sourceFile.TryTo.Text.If(out var source, out var exception))
				{
					var stopwatch = new Stopwatch();
					if (Stopwatch)
					{
						stopwatch.Start();
					}

					var configuration = new CompilerConfiguration { ShowOperations = ShowOps, Tracing = Trace };
					var compiler = new Compiler(source, configuration, this);
					var result =
						from machine in compiler.Generate().OnSuccess(m =>
						{
							if (configuration.ShowOperations)
							{
								WriteLine(m.Operations);
							}
						})
						from executed in machine.Execute()
						select executed;
					if (result.IfNot(out var failureException))
					{
						WriteLine($"Exception: {failureException}");
					}

					if (Stopwatch)
					{
						stopwatch.Stop();
						WriteLine(stopwatch.Elapsed.ToLongString(true));
					}
				}
				else
				{
					WriteLine($"Exception: {exception}");
				}
			}
		}

		public bool Exec { get; set; }

		public IMaybe<FileName> File { get; set; } = none<FileName>();

		public bool Stopwatch { get; set; }

		public bool ShowOps { get; set; }

		public bool Trace { get; set; }

		public void Print(string value)
		{
			putter.Reset();
			Write(value);
		}

		public void PrintLine(string value)
		{
			putter.Reset();
			WriteLine(value);
		}

		public void Put(string value) => Write(putter.Put(value));

		public IResult<string> ReadLine()
		{
			var line = Console.ReadLine();
			return assert(line != null, () => line, "Input cancelled");
		}

		public bool Cancelled() => KeyAvailable && ReadKey().Key == ConsoleKey.Escape;

		public void Peek(string message, int index) { }
	}
}