using Core.Monads;
using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class EnumMemberClass : UserClass
{
   public EnumMemberClass(string className, string parentClassName) : base(className, parentClassName)
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

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      RegisterMessage("string".get(), (_, _) => (KString)className.Replace("$", "."));
   }

   public override bool AssignCompatible(BaseClass otherClass) =>
      otherClass is EnumMemberClass otherEnumMember && parentClassName == otherEnumMember.parentClassName;
}