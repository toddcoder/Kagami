namespace Kagami.Library.Invokables
{
   public class ExpressionInvokable : IInvokable
   {
      public ExpressionInvokable(string image)
      {
         Image = image;
         Index = -1;
         Address = -1;
      }

      public int Index { get; set; }

      public int Address { get; set; }

      public Parameters Parameters => new();

      public string Image { get; }

      public bool Constructing => false;
   }
}