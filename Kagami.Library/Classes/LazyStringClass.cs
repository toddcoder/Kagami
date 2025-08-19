using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class LazyStringClass : BaseClass
{
   public override string Name => "LazyString";

   public override IObject DefaultValue => new LazyString("");

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      collectionMessages();
      sliceableMessages();
      compareMessages();
      rangeMessages();
      textFindingMessages();
   }

   public override IObject DynamicInvoke(IObject obj, Message message)
   {
      var @string = (KString)obj.AsString;
      var @class = ObjectFunctions.classOf(@string);

      return @class.SendMessage(@string, message);
   }
}