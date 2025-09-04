using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Classes.ClassFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Classes;

public class JunctionClass : BaseClass
{
   public override string Name => "Junction";

   public override IObject DefaultValue => Junction.Empty;

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      messages.Remove("numberize()");
      registerMessage("flatten()", (obj, _) => function<Junction>(obj, j => j.Flatten()));
   }

   public override IObject DynamicInvoke(IObject obj, Message message)
   {
      if (obj is Junction junction)
      {
         if (message.Arguments.Length > 0 && message.Arguments[0] is Junction junction2)
         {
            List<IObject> result = [];
            foreach (var item1 in junction.Items)
            {
               foreach (var item2 in junction2.Items)
               {
                  Selector newSelector = message.Selector.ToString().Replace("<Junction>", "");
                  var newMessage = new Message(newSelector, [item2, .. message.Arguments.Value.Skip(1)]);
                  var @class = classOf(item1);
                  result.Add(@class.SendMessage(item1, newMessage));
               }
            }

            return junction.NewJunction(result).Flatten();
         }
         else
         {
            return junction.Apply(message).Flatten();
         }
      }
      else
      {
         throw messageNotFound(this, message.Selector.ToString());
      }
   }
}