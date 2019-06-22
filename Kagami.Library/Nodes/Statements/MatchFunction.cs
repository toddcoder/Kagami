using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Runtime;
using Core.Booleans;
using Core.Strings;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Nodes.Statements
{
   public class MatchFunction : Statement
   {
      Selector selector;
      Parameters parameters;
      Block block;
      bool overriding;
      string className;

      public MatchFunction(string functionName, Parameters parameters, If ifStatement, bool overriding, string className)
      {
         selector = parameters.Selector(functionName);
         this.parameters = parameters;
         block = new Block(ifStatement);
         this.overriding = overriding;
         this.className = className;
      }

      public IInvokable getInvokable() => new FunctionInvokable(selector, parameters, ToString());

      public override void Generate(OperationsBuilder builder)
      {
         var invokable = getInvokable();
         var lambda = new Lambda(invokable);
         if (builder.RegisterInvokable(invokable, block, overriding).If(out _, out var exception))
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
	         throw exception;
         }

         if (className.IsNotEmpty())
         {
	         if (Module.Global.Class(className).If(out var cls))
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

      public void Deconstruct(out Selector selector, out Parameters parameters, out Block block, out bool yielding,
         out IInvokable invokable, out bool overriding)
      {
         selector = this.selector;
         parameters = this.parameters;
         block = this.block;
         yielding = false;
         invokable = getInvokable();
         overriding = this.overriding;
      }
   }
}