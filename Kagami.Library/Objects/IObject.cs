using Standard.Types.Collections;

namespace Kagami.Library.Objects
{
   public interface IObject
   {
      string ClassName { get; }

      string AsString { get; }

      string Image { get; }

      int Hash { get; }

      bool IsEqualTo(IObject obj);

      bool Match(IObject comparisand, Hash<string, IObject> bindings);

      bool IsTrue { get; }
   }
}