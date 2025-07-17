using Kagami.Library.Packages;

namespace Kagami.Library.Runtime;

public class GlobalFrame : Frame
{
   public GlobalFrame()
   {
      Sys = new Sys();
      Sys.LoadTypes(Module.Global);
      fields.New("sys", FieldType.Package, Sys);

      Math = new KMath();
      Sys.LoadTypes(Module.Global);
      fields.New("math", FieldType.Package, Math);

      IO = new IO();
      IO.LoadTypes(Module.Global);
      fields.New("io", FieldType.Package, IO);
   }

   public Sys Sys { get; }

   public KMath Math { get; }

   public IO IO { get; }
}