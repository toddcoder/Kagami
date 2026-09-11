using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Classes;

public class StatisticsClass : BaseClass
{
   public override string Name => "Statistics";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      registerMessage("sum".get(), (obj, _) => function<Statistics>(obj, statistics => statistics.Sum));
      registerMessage("count".get(), (obj, _) => function<Statistics>(obj, statistics => statistics.Count));
      registerMessage("average".get(), (obj, _) => function<Statistics>(obj, statistics => statistics.Average));
      registerMessage("min".get(), (obj, _) => function<Statistics>(obj, statistics => someOf(statistics.Min)));
      registerMessage("max".get(), (obj, _) => function<Statistics>(obj, statistics => someOf(statistics.Max)));
   }

   public override IObject DefaultValue => new Statistics(KArray.Empty.GetIterator(false));
}