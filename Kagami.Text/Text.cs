using Kagami.Library.Packages;
using Kagami.Library.Runtime;

namespace Kagami.Text
{
   public class Text : Package
   {
      public override string ClassName => "Text";

      public override void LoadTypes(Module module)
      {
         module.RegisterClass(new TextClass());
         module.RegisterClass(new StringBufferClass());
      }

      public StringBuffer StringBuffer() => new StringBuffer();

      public StringBuffer StringBuffer(string initialValue) => new StringBuffer(initialValue);
   }
}