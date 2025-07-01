using Kagami.Library.Packages;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Plots;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public class PlotsClass : PackageClass
{
   public override string Name => "Plots";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      registerPackageFunction("Plot()", (obj, _) => function<Plots>(obj, p => p.Plot()));
   }
}