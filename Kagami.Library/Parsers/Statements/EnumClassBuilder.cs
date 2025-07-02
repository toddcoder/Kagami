using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;

namespace Kagami.Library.Parsers.Statements;

public class EnumClassBuilder : ClassBuilder
{
   public EnumClassBuilder(string className) : base(className, Parameters.Empty, "", [], false, new Block())
   {
   }

   public override UserClass CreateUserClass() => new EnumClass(className, "");
}