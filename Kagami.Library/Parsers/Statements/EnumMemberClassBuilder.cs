using Core.Monads;
using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;

namespace Kagami.Library.Parsers.Statements;

public class EnumMemberClassBuilder : ClassBuilder
{
   public EnumMemberClassBuilder(string className, Parameters parameters, string parentClassName, Block commonBlock) : base(className, parameters,
      parentClassName, [], false, commonBlock)
   {
   }

   public required Selector Selector { get; set; }

   public required Maybe<IObject> Ordinal { get; set; }

   public override UserClass CreateUserClass() => new EnumMemberClass(className, parentClassName) { Selector = Selector, Ordinal = Ordinal };
}