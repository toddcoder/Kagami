using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Packages
{
   public abstract class Package : IObject
   {
      protected Fields fields;

      public Package() => fields = new Fields();

      public Fields Fields => fields;

      public abstract string ClassName { get; }

      public string AsString => ClassName;

      public string Image => ClassName;

      public int Hash => ClassName.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj.ClassName == ClassName;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => true;

      public abstract void LoadTypes(Module module);
   }
}