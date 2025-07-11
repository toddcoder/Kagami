using Core.Enumerables;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class Pattern : Statement
{
   protected string name;
   protected Parameters parameters;
   protected Block block;
   protected string image;

   public Pattern(string name, Parameters parameters, Block block)
   {
      this.name = name;
      this.parameters = parameters;
      this.block = block;
      image = $"{name}({parameters.Select(_ => "_").ToString(",")})";
   }

   public override void Generate(OperationsBuilder builder)
   {
      var invokable = new FunctionInvokable(name, parameters, name);
      var _index = builder.RegisterInvokable(invokable, block, true);
      if (_index)
      {
         var lambda = new Lambda(invokable, false);
         var pattern = new Objects.Pattern(name, lambda, parameters);
         builder.NewField(name, false, true);
         builder.PushObject(pattern);
         builder.AssignField(name, true);
      }
      else
      {
         throw _index.Exception;
      }
   }

   public override string ToString() => image;
}