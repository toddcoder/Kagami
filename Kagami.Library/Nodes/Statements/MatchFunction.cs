using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Runtime;
using Core.Booleans;
using Core.Strings;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Nodes.Statements;

public class MatchFunction : Statement
{
   protected Selector selector;
   protected Parameters parameters;
   protected Block block;
   protected bool overriding;
   protected string className;

   public MatchFunction(string functionName, Parameters parameters, If ifStatement, bool overriding, string className)
   {
      selector = parameters.Selector(functionName);
      this.parameters = parameters;
      block = new Block(ifStatement) { new ReturnNothing() };
      block.AddReturnIf();
      this.overriding = overriding;
      this.className = className;
   }

   public IInvokable getInvokable() => new FunctionInvokable(selector, parameters, ToString());

   public override void Generate(OperationsBuilder builder)
   {
      var invokable = getInvokable();
      var lambda = new Lambda(invokable);
      var _index = builder.RegisterInvokable(invokable, block, overriding);
      if (_index)
      {
         if (!overriding)
         {
            builder.NewSelector(selector, false, true);
         }

         builder.PushObject(lambda);
         builder.Peek(Index);
         builder.AssignField(selector, overriding);
      }
      else
      {
         throw _index.Exception;
      }

      if (className.IsNotEmpty())
      {
         if (Module.Global.Class(className) is (true, var cls))
         {
            cls.RegisterMessage(selector, (obj, msg) => BaseClass.Invoke(obj, msg.Arguments, lambda));
         }
         else
         {
            throw classNotFound(className);
         }
      }
   }

   public override string ToString() => $"{overriding.Extend("override ")}match {selector.Image}() ...";

   public void Deconstruct(out Selector selector, out Parameters parameters, out Block block, out bool yielding, out IInvokable invokable,
      out bool overriding)
   {
      selector = this.selector;
      parameters = this.parameters;
      block = this.block;
      yielding = false;
      invokable = getInvokable();
      overriding = this.overriding;
   }
}