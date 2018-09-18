using System.Linq;
using Kagami.Library.Objects;
using Standard.Computer;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using Standard.Types.Maybe;
using Standard.Types.Numbers;
using Standard.Types.Strings;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Awk.AwkFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Awk
{
	public class Awkifier : IObject, ICollection
	{
		FileName file;
		Regex recordPattern;
		Regex fieldPattern;
		string[] records;
		string[] fields;
		string recordSeparator;
		string fieldSeparator;
		bool recordsCreated;

		public Awkifier(FileName file)
		{
			this.file = file;

			recordPattern = new Regex("(/r /n | /r | /n)", false, false, false, false);
			fieldPattern = new Regex("/s+", false, false, false, false);
			recordSeparator = "\r\n";
			fieldSeparator = " ";
			recordsCreated = false;
		}

		public Regex RecordPattern
		{
			get => recordPattern;
			set
			{
				recordPattern = value;
				recordsCreated = false;
			}
		}

		public Regex FieldPattern
		{
			get => fieldPattern;
			set
			{
				fieldPattern = value;
				recordsCreated = false;
			}
		}

		public String RecordSeparator
		{
			get => recordSeparator;
			set
			{
				recordSeparator = value.AsString;
				recordsCreated = false;
			}
		}

		public String FieldSeparator
		{
			get => fieldSeparator;
			set
			{
				fieldSeparator = value.AsString;
				recordsCreated = false;
			}
		}

		public String this[int index]
		{
			get
			{
				if (index.Between(0).Until(getLength()))
					return fields[index];
				else
					return "";
			}
		}

		public string ClassName => "Awkifier";

		public string AsString
		{
			get
			{
				split(0);
				return records.Listify(recordSeparator).Elliptical(80, '\n');
			}
		}

		public string Image => AsString;

		public int Hash
		{
			get
			{
				var hash = 17;
				hash += 37 * file.GetHashCode();
				hash += 37 * recordPattern.Hash;
				hash += 37 * fieldPattern.Hash;

				return hash;
			}
		}

		public bool IsEqualTo(IObject obj)
		{
			return obj is Awkifier awkifier && file == awkifier.file && recordPattern.IsEqualTo(awkifier.recordPattern) &&
				fieldPattern.IsEqualTo(awkifier.fieldPattern);
		}

		public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

		public bool IsTrue => file.Exists();

		void splitIfRecordNotCreated()
		{
			if (!recordsCreated)
			{
				records = regexSplit(file.Text, recordPattern);
				recordsCreated = true;
         }
		}

		void split(int index)
		{
			splitIfRecordNotCreated();

			var record = records[index];
			fields = awkSplit(record, fieldPattern);
		}

		public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

		public IMaybe<IObject> Next(int index)
		{
			if (index < records.Length)
			{
				split(index);
            //var awkRecord = new AwkRecord(fields, fieldPattern, fieldSeparator);
				//return awkRecord.Some<IObject>();
				return new Tuple(fields.Select(String.StringObject).ToArray()).Some<IObject>();
			}
			else
				return none<IObject>();
		}

		public IMaybe<IObject> Peek(int index) => Next(index);

		int getLength()
		{
			splitIfRecordNotCreated();
			return records.Length;
		}

		public Int Length => getLength();

		public bool ExpandForArray => true;

		public Boolean In(IObject item)
		{
			var subject = item.AsString;
			splitIfRecordNotCreated();

         return records.Contains(subject);
		}

		public Boolean NotIn(IObject item) => !In(item).Value;

		public IObject Times(int count) => this;

		public IIterator GetIndexedIterator() => new IndexedIterator(this);

		public IIterator If(Regex regex)
		{
			return (IIterator)new LazyIterator(this).If(new InternalLambda(args =>
			{
				var input = ((String)args[0]).Value;
				var result = regex.IsMatch(input);
				return (IObject)result;
			}));
		}
	}
}