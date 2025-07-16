using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class EnumMemberClass : UserClass
{
   public EnumMemberClass(string className) : base(className, "")
   {
   }

   public required Selector Selector { get; set; }

   public required Maybe<IObject> Ordinal
   {
      get;
      set
      {
         field = value;
         if (value is (true, var actualValue))
         {
            RegisterMessage("ordinal".get(), (_, _) => actualValue);
         }
      }
   }

   protected string asString(UserObject userObject)
   {
      var name = plainName(userObject);
      var parameters = userObject.ParameterValues.Select(p => p.AsString).ToString(", ");
      if (parameters.Length > 0)
      {
         parameters = $"({parameters})";
      }

      return $"{name}{parameters}";
   }

   protected static string plainName(IObject obj) => plainName((UserObject)obj);

   protected static string plainName(UserObject userObject) => userObject.ClassName.Substitute(@"^.*\$(.+)$; u", "$1");

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      RegisterMessage("string".get(), (obj, _) => (KString)asString((UserObject)obj));
      RegisterMessage("name".get(), (obj, _) => (KString)plainName(obj));
      RegisterMessage("hash".get(), (obj, _) => (Int)plainName(obj).GetHashCode());
      RegisterMessage("equal(_)", (obj, msg) => (KBoolean)(plainName(obj) == plainName(msg.Arguments[0])));
   }

   public override bool AssignCompatible(BaseClass otherClass) =>
      otherClass is EnumMemberClass otherEnumMember && parentClassName == otherEnumMember.parentClassName;
}