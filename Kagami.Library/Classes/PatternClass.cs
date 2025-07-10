using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class PatternClass : BaseClass
{
   public override string Name => "Pattern";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      messages["with(_<Dictionary>)"] = (obj, msg) => function<Pattern, Dictionary>(obj, msg, (p, d) => p.With(d));
      messages["(_)"] = (obj, msg) => function<Pattern>(obj, p => p.Invoke(msg.Arguments));
   }
}