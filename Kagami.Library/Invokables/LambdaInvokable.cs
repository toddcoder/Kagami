namespace Kagami.Library.Invokables
{
   public class LambdaInvokable : IInvokable
   {
      public LambdaInvokable(Parameters parameters, string image)
      {
         Parameters = parameters;
         Image = image;
      }

      public int Index { get; set; } = -1;

      public int Address { get; set; } = -1;

      public Parameters Parameters { get; }

      public string Image { get; }

      public bool Constructing => false;

      public bool IsUserInvokable => true;

      public override string ToString() => Image;
   }
}