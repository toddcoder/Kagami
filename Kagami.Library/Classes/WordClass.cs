using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class WordClass : BaseClass
{
   public override string Name => "Word";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      registerMessage("text".get(), (obj, _) => function<Word>(obj, w => KString.StringObject(w.Text)));
      registerMessage("index".get(), (obj, _) => function<Word>(obj, w => Int.IntObject(w.Index)));
   }
}