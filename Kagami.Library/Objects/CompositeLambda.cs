namespace Kagami.Library.Objects;

public class CompositeLambda : Lambda
{
   protected Lambda lambda1;
   protected Lambda lambda2;

   public CompositeLambda(Lambda lambda1, Lambda lambda2) : base(lambda1.Invokable, true)
   {
      this.lambda1 = lambda1;
      this.lambda2 = lambda2;
   }

   public override IObject Invoke(params IObject[] arguments)
   {
      var result = lambda1.Invoke(arguments);
      return lambda2.Invoke(result);
   }

   public override string AsString => $"{lambda1.AsString} >> {lambda2.AsString}";

   public override string Image => $"{lambda1.Image} >> {lambda2.Image}";
}