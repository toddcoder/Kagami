using Standard.Types.Collections;

namespace Kagami.Library.Objects
{
   public struct Binding : IObject
   {
      string name;
      IObject value;

      public Binding(string name, IObject value) : this()
      {
         this.name = name;
         this.value = value;
      }

      public string Name => name;

      public IObject Value => value;

      public string ClassName => "Binding";

      public string AsString => value.AsString;

      public string Image => $"{name} @ {value.Image}";

      public int Hash => Image.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Binding b && name == b.name && value.IsEqualTo(b.value);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => value.Match(comparisand, bindings);

      public bool IsTrue => value.IsTrue;
   }
}