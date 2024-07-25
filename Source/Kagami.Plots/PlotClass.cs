using Kagami.Library.Classes;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Plots
{
	public class PlotClass : BaseClass
	{
		public override string Name => "Plot";

		public override void RegisterMessages()
		{
			base.RegisterMessages();

			messages["line(_<Int>,_<Int>,_<Int>,_<Int>)"] = (obj, msg) =>
				{
					return function<Plot, Int, Int, Int, Int>(obj, msg,
						(p, x1, y1, x2, y2) => p.Line(x1.Value, y1.Value, x2.Value, y2.Value));
				};
		}
	}
}