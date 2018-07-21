using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class DataComparisandClass : BaseClass
   {
      string className;
      string name;

      public DataComparisandClass(string className, string name)
      {
         this.className = className;
         this.name = name;
      }

      public override string Name => $"{className}.{name}";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         messages["name".get()] = (obj, msg) => function<DataComparisand>(obj, dc => dc.Name);
         messages["comparisands".get()] = (obj, msg) => function<DataComparisand>(obj, dc => dc.Comparisands);
         messages["ordinal".get()] = (obj, msg) => function<DataComparisand>(obj, dc => dc.Ordinal);
      }

      public override bool MatchCompatible(BaseClass otherClass)
      {
         if (otherClass is DataTypeClass dataTypeClass)
            return dataTypeClass.Name == className;
         else
            return base.MatchCompatible(otherClass);
      }

      public override bool AssignCompatible(BaseClass otherClass)
      {
         if (otherClass is DataComparisandClass dataComparisandClass)
            return className == dataComparisandClass.className;
         else
            return false;
      }
   }
}