namespace Kagami.Library.Invokables;

public struct RequireFunction : IInvokable
{
   public int Index { get; set; }
   public int Address { get; set; }
   public Parameters Parameters => Parameters.Empty;
   public string Image => "requireFunction";
   public bool Constructing => false;
}