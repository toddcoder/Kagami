using System.IO;
using System.Windows.Forms;
using Kagami.Library;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Kagami.Playground
{
	public class PlaygroundContext : IContext
	{
		TextWriter writer;
		TextReader reader;
		Hash<int, string> peeks;
		bool cancelled;
		Putter putter;

		public PlaygroundContext(TextWriter writer, TextReader reader)
		{
			this.writer = writer;
			this.reader = reader;
			peeks = new AutoHash<int, string>(k => "");
			cancelled = false;
			putter = new Putter();
		}

		public Machine Machine { get; set; }

		public Hash<int, string> Peeks => peeks;

		public void Print(string value)
		{
			putter.Reset();
			writer.Write(value);
		}

		public void PrintLine(string value)
		{
			putter.Reset();
			writer.WriteLine(value);
		}

		public void Put(string value) => writer.Write(putter.Put(value));

		public IResult<string> ReadLine()
		{
			putter.Reset();
			var line = reader.ReadLine();

			return assert(line != null, () => line, "Input cancelled");
		}

		public bool Cancelled()
		{
			Application.DoEvents();
			return cancelled;
		}

		public void Peek(string message, int index) => peeks[index] = message;

		public void ClearPeeks() => peeks.Clear();

		public void Cancel() => cancelled = true;

		public void Reset()
		{
			cancelled = false;
			putter.Reset();
		}
	}
}