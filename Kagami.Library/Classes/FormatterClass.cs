using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class FormatterClass : BaseClass
{
   public override string Name => "Formatter";

   public override IObject DefaultValue => new Formatter(new LazyString(""), KArray.Empty);

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