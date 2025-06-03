using Kagami.Library.Objects;

namespace Kagami.Library.Invokables;

public class FunctionInvokable : IInvokable
{
   protected Selector selector;

   public FunctionInvokable(Selector selector, Parameters parameters, string image)
   {
      this.selector = selector;
      Parameters = parameters;
      Image = image;
   }

   public Selector Selector => selector;

   public int Index { get; set; } = -1;

   public int Address { get; set; } = -1;

   public Parameters Parameters { get; }

   public string Image { get; }

   public virtual bool Constructing => false;

   public bool IsUserInvokable => true;

   public override string ToString() => Image;
}