using Core.Collections;
using Core.Enumerables;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct DataComparisand : IObject
{
   private readonly string className;
   private readonly string name;
   private readonly IObject[] comparisands;
   private readonly IObject ordinal;

   public DataComparisand(string className, string name, IObject[] comparisands, IObject ordinal) : this()
   {
      this.className = className;
      this.name = name;
      this.comparisands = comparisands;
      this.ordinal = ordinal;
   }

   public KString Name => name;

   public KTuple Comparisands => new(comparisands);

   public IObject Ordinal => ordinal;

   public string ClassName => $"{className}.{name}";

   public string AsString => name;

   public string Image => comparisands.Length == 0 ? name : $"{name}({comparisands.Select(c => c.Image).ToString(", ")})";

   public int Hash => comparisands.GetHashCode();

   private bool comparisandsEqual(IObject[] others)
   {
      if (comparisands.Length == others.Length)
      {
         var self = this;
         return Enumerable.Range(0, comparisands.Length).Select(i => self.comparisands[i].IsEqualTo(others[i])).All(b => b);
      }
      else
      {
         return false;
      }
   }

   public bool IsEqualTo(IObject obj)
   {
      return obj is DataComparisand dc && className == dc.className && ordinal.IsEqualTo(dc.ordinal) && comparisandsEqual(dc.comparisands);
   }

   private bool comparisandsMatch(IObject[] others, Hash<string, IObject> bindings)
   {
      var self = this;
      return Enumerable.Range(0, comparisands.Length).Select(i => self.comparisands[i].Match(others[i], bindings)).All(b => b);
   }

   public bool Match(IObject comparisand, Hash<string, IObject> bindings)
   {
      return match(this, comparisand, (dc1, dc2) => dc1.comparisandsMatch(dc2.comparisands, bindings), bindings);
   }

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();
}