using System.Linq;
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
      var skip1Parameters = new Parameters(parameters.GetParameters().Skip(1).ToArray());
      var comparisandParameters = new Parameters(parameters.GetParameters().Take(1).ToArray());
      var invokable = new FunctionInvokable(name, comparisandParameters, name);
      var _index = builder.RegisterInvokable(invokable, block, true);
      if (_index)
      {
         var lambda = new Lambda(invokable);
         var pattern = new Objects.Pattern(name, lambda, skip1Parameters);
         builder.NewField(name, false, true);
         builder.PushObject(pattern);
         builder.Peek(Index);
         builder.AssignField(name, true);
      }
      else
      {
         throw _index.Exception;
      }
   }

   public override string ToString() => image;
}