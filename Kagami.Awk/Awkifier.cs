using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Computers;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Awk.AwkFunctions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;
using CollectionFunctions = Kagami.Library.Objects.CollectionFunctions;

namespace Kagami.Awk;

public class Awkifier : IObject, ICollection
{
   protected string source;
   protected Regex recordPattern = new("(/r /n | /r | /n)", false, false, false, false);
   protected Regex fieldPattern = new("/s+", false, false, false, false);
   protected string[] records = [];
   protected string[] fields = [];
   protected string recordSeparator = "/r/n";
   protected string fieldSeparator = " ";
   protected bool recordsCreated;

   public Awkifier(string source, bool asFile)
   {
      this.source = asFile ? ((FileName)source).Text : source;
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

   public KString RecordSeparator
   {
      get => recordSeparator;
      set
      {
         recordSeparator = value.AsString;
         recordsCreated = false;
      }
   }

   public KString FieldSeparator
   {
      get => fieldSeparator;
      set
      {
         fieldSeparator = value.AsString;
         recordsCreated = false;
      }
   }

   public KString this[int index] => index.Between(0).Until(getLength()) ? fields[index] : "";

   public string ClassName => "Awkifier";

   public string AsString
   {
      get
      {
         split(0);
         return records.ToString(recordSeparator).Elliptical(80, '\n');
      }
   }

   public string Image => AsString;

   public int Hash => HashCode.Combine(source, recordPattern, fieldPattern);

   public bool IsEqualTo(IObject obj)
   {
      return obj is Awkifier awkifier && source == awkifier.source && recordPattern.IsEqualTo(awkifier.recordPattern) &&
         fieldPattern.IsEqualTo(awkifier.fieldPattern);
   }

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => source.IsNotEmpty();

   public Guid Id { get; init; } = Guid.NewGuid();

   protected void splitIfRecordNotCreated()
   {
      if (!recordsCreated)
      {
         records = regexSplit(source, recordPattern);
         recordsCreated = true;
      }
   }

   protected void split(int index)
   {
      splitIfRecordNotCreated();

      var record = records[index];
      fields = awkSplit(record, fieldPattern);
   }

   public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

   public Maybe<IObject> Next(int index)
   {
      if (index < records.Length)
      {
         split(index);
         var array = fields.Select(KString.StringObject).ToArray();
         var currentFields = Machine.Fields;
         for (var i = 0; i < array.Length; i++)
         {
            currentFields.New($"__${i}", array[i]);
         }

         return new KTuple(array).Some<IObject>();
      }
      else
      {
         return nil;
      }
   }

   public Maybe<IObject> Peek(int index) => Next(index);

   protected int getLength()
   {
      splitIfRecordNotCreated();
      return records.Length;
   }

   public Int Length => getLength();

   public bool ExpandForArray => true;

   public KBoolean In(IObject item)
   {
      var subject = item.AsString;
      splitIfRecordNotCreated();

      return records.Contains(subject);
   }

   public KBoolean NotIn(IObject item) => !In(item).Value;

   public IObject Times(int count) => this;

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public IIterator If(Regex regex)
   {
      return (IIterator)new LazyIterator(this).If(new InternalLambda(args =>
      {
         var input = ((KString)args[0]).Value;
         var result = regex.IsMatch(input);
         return result;
      }));
   }

   public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);
}