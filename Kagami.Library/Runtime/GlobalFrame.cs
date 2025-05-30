﻿using Kagami.Library.Packages;

namespace Kagami.Library.Runtime;

public class GlobalFrame : Frame
{
   public GlobalFrame()
   {
      Sys = new Sys();
      Sys.LoadTypes(Module.Global);
      fields.New("sys", Sys);

      Math = new KMath();
      Sys.LoadTypes(Module.Global);
      fields.New("math", Math);
   }

   public Sys Sys { get; }

   public KMath Math { get; }
}