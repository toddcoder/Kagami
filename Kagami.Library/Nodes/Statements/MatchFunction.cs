using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Runtime;
using Core.Booleans;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Nodes.Statements;

public class MatchFunction : Statement, IOverridable
{
   protected Selector selector;
   protected Parameters parameters;
   protected bool isHidden;
   protected Block block;
   protected bool overriding;
   protected string className;

   public MatchFunction(string functionName, Parameters parameters, bool isHidden, If ifStatement, Maybe<TypeConstraint> _typeConstraint,
      bool overriding, string className)
   {
      selector = parameters.Selector(functionName);
      this.parameters = parameters;
      this.isHidden = isHidden;
      block = new Block(ifStatement, _typeConstraint) { new ReturnNothing() };
      this.overriding = overriding;
      this.className = className;
   }

   public IInvokable getInvokable() => new FunctionInvokable(selector, parameters, ToString());

   public Selector Selector => selector;

   public override void Generate(OperationsBuilder builder)
   {
      if (SelfAlias.IsNotEmpty())
      {
         block.InsertSelfAlias(SelfAlias);
      }

      var invokable = getInvokable();
      var lambda = new Lambda(invokable, true);
      var _index = builder.RegisterInvokable(invokable, block, overriding);
      if (_index)
      {
         if (!overriding)
         {
            builder.NewSelector(selector, false, true);
         }

         builder.PushObject(lambda);
         builder.AssignField(selector, overriding);
      }
      else
      {
         throw _index.Exception;
      }

      if (className.IsNotEmpty())
      {
         if (Module.Global.Value.Class(className) is (true, var cls))
         {
            cls.RegisterMessage(selector, (obj, msg) => BaseClass.Invoke(obj, msg.Arguments, lambda, false));
         }
         else
         {
            throw classNotFound(className);
         }
      }
   }

   public override string ToString() => $"{overriding.Extend("override ")}match {selector.Image}() ...";

   public void SetToOverriding() => overriding = true;

   public void Deconstruct(out Selector selector, out Parameters parameters, out Block block, out bool yielding, out IInvokable invokable,
      out bool overriding, out bool isHidden)
   {
      selector = this.selector;
      parameters = this.parameters;
      block = this.block;
      yielding = false;
      invokable = getInvokable();
      overriding = this.overriding;
      isHidden = this.isHidden;
   }

   public bool IsFixed { get; set; }

   public string SelfAlias { get; set; } = "";
}