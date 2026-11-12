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
      registerMessage("prefix".get(), (obj, _) => function<Word>(obj, w => KString.StringObject(w.Prefix)));
      registerMessage("suffix".get(), (obj, _) => function<Word>(obj, w => KString.StringObject(w.Suffix)));
      registerMessage("index".get(), (obj, _) => function<Word>(obj, w => Int.IntObject(w.Index)));
      registerMessage("length".get(), (obj, _) => function<Word>(obj, w => Int.IntObject(w.Length)));
   }

   public override IObject DefaultValue => new Word("", "", "", -1, 0);
}