using Kagami.Library.Packages;

namespace Kagami.Library.Runtime
{
   public class GlobalFrame : Frame
   {
      public GlobalFrame()
      {
         Sys = new Sys();
         fields.New("sys", Sys);

         Math = new Math();
         fields.New("math", Math);
      }

      public Sys Sys { get; }

      public Math Math { get; }
   }
}