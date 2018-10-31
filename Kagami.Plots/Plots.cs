using Kagami.Library.Packages;
using Kagami.Library.Runtime;

namespace Kagami.Plots
{
	public class Plots : Package
	{
		public override string ClassName => "Plots";

		public override void LoadTypes(Module module)
		{
			module.RegisterClass(new PlotsClass());
			module.RegisterClass(new PlotClass());
		}

		public Plot Plot() => new Plot();
	}
}