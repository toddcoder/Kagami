using Kagami.Library;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Text;

public class StringBufferClass : BaseClass
{
   public override string Name => "StringBuffer";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      messages["[]"] = (obj, msg) => function<StringBuffer, Int>(obj, msg, (sb, i) => sb[i.Value]);
      messages["[]="] = (obj, msg) => function<StringBuffer, Int, KChar>(obj, msg, (sb, i, c) => sb[i.Value] = c);
      messages["<<"] = (obj, msg) => function<StringBuffer, IObject>(obj, msg, (sb, o) => sb.Append(o));
      messages["length".get()] = (obj, _) => function<StringBuffer>(obj, sb => sb.Length);
      messages["clear"] = (obj, _) => function<StringBuffer>(obj, sb => sb.Clear());
   }
}