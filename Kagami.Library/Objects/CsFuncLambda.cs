using Kagami.Library.Packages;

namespace Kagami.Library.Objects;

public class CsFuncLambda : Lambda
{
   protected Package package;
   protected string name;
   protected Func<IObject, Message, IObject> function;

   public CsFuncLambda(Package package, string name, Func<IObject, Message, IObject> function) : base(new RuntimeInvokable(0, ""), false)
   {
      this.package = package;
      this.name = name;
      this.function = function;
   }

   public override IObject Invoke(params IObject[] arguments)
   {
      var argumentsObject = new Arguments(arguments);
      var selector = argumentsObject.Selector(name);
      var message = new Message(selector, arguments);

      return function.Invoke(package, message);
   }
}