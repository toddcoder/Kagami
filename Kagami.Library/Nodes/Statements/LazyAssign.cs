using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements;

public class LazyAssign(string fieldName, Block block, bool isHidden, bool isOverride) : Statement, IFieldStatement
{
   protected string lazyName = lazyName(fieldName);

   public string FieldName => fieldName;

   public bool IsHidden => isHidden;

   public bool IsOverride => isOverride;

   public override void Generate(OperationsBuilder builder)
   {
      var invokable = new LambdaInvokable(Parameters.Empty, block.ToString());
      var _index = builder.RegisterInvokable(invokable, block, true);
      if (_index)
      {
         builder.NewLambda(invokable, true);
         builder.StoreField(lazyName, false, true, isOverride, nil);
         builder.PushObject(new Objects.Singleton());
         builder.StoreField(fieldName, true, true, isOverride, nil);
      }
      else
      {
         throw _index.Exception;
      }
   }

   public override string ToString() => $"lazy {fieldName} = {block}";

   public string Name => fieldName;

   public bool Mutable => false;

   public Maybe<TypeConstraint> TypeConstraint => nil;
}