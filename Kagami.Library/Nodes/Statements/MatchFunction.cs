using System.Linq;
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
      IObject[] comparisands;
      Block block;
      bool overriding;
      string className;

      public MatchFunction(string functionName, IObject[] comparisands, Block block, bool overriding, string className)
      {
	      selector = new Selector(functionName, new SelectorItem[0], functionName);
	      this.comparisands = comparisands;
         this.overriding = overriding;
         this.className = className;
      }

      public IInvokable getInvokable() => new FunctionInvokable(selector, new Parameters(comparisands.Length), ToString());

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