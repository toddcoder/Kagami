using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Runtime;
using Standard.Types.Booleans;
using Standard.Types.Strings;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Nodes.Statements
{
   public class MatchFunction : Statement
   {
      string functionName;
      Parameters parameters;
      Block block;
      bool overriding;
      string className;

      public MatchFunction(string functionName, Parameters parameters, If ifStatement, bool overriding, string className)
      {
         this.functionName = functionName;
         this.parameters = parameters;
         block = new Block(ifStatement);
         this.overriding = overriding;
         this.className = className;
      }

      public IInvokable getInvokable() => new FunctionInvokable(functionName, parameters, ToString());

      public override void Generate(OperationsBuilder builder)
      {
         var invokable = getInvokable();
         var lambda = new Lambda(invokable);
         if (builder.RegisterInvokable(invokable, block, overriding).If(out _, out var exception))
         {
            if (!overriding)
               builder.NewField(functionName, false, true);
            builder.PushObject(lambda);
            builder.Peek(Index);
            builder.AssignField(functionName, overriding);
         }
         else
            throw exception;

         if (className.IsNotEmpty())
            if (Module.Global.Class(className).If(out var cls))
               cls.RegisterMessage(functionName, (obj, msg) => BaseClass.Invoke(obj, msg.Arguments, lambda));
            else
               throw classNotFound(className);
      }

      public override string ToString() => $"{overriding.Extend("override ")}match {functionName}() ...";

      public void Deconstruct(out string functionName, out Parameters parameters, out Block block, out bool yielding,
         out IInvokable invokable, out bool overriding)
      {
         functionName = this.functionName;
         parameters = this.parameters;
         block = this.block;
         yielding = false;
         invokable = getInvokable();
         overriding = this.overriding;
      }
   }
}