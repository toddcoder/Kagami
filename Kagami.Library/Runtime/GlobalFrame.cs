using Kagami.Library.Packages;

namespace Kagami.Library.Runtime
{
   public class GlobalFrame : Frame
   {
      public GlobalFrame()
      {
         fields.New("sys", new Sys());
         fields.New("math", new Math());
      }
   }
}