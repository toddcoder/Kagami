namespace Kagami.Library.Invokables
{
   public class ExpressionInvokable : IInvokable
   {
      public ExpressionInvokable(string image) => Image = image;

      public int Index { get; set; } = -1;

      public int Address { get; set; } = -1;

      public Parameters Parameters => new Parameters();

      public string Image { get; }

      public bool Constructing => false;
   }
}