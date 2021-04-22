using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class DataComparisandClass : BaseClass
   {
      protected string className;
      protected string name;

      public DataComparisandClass(string className, string name)
      {
         this.className = className;
         this.name = name;
      }

      public override string Name => $"{className}.{name}";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         messages["name".get()] = (obj, _) => function<DataComparisand>(obj, dc => dc.Name);
         messages["comparisands".get()] = (obj, _) => function<DataComparisand>(obj, dc => dc.Comparisands);
         messages["ordinal".get()] = (obj, _) => function<DataComparisand>(obj, dc => dc.Ordinal);
      }

      public override bool MatchCompatible(BaseClass otherClass)
      {
         if (otherClass is DataTypeClass dataTypeClass)
         {
            return dataTypeClass.Name == className;
         }
         else
         {
            return base.MatchCompatible(otherClass);
         }
      }

      public override bool AssignCompatible(BaseClass otherClass)
      {
         return otherClass is DataComparisandClass dataComparisandClass && className == dataComparisandClass.className;
      }
   }
}