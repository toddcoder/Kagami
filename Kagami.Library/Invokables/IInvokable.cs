namespace Kagami.Library.Invokables;

public interface IInvokable
{
   int Index { get; set; }

   int Address { get; set; }

   Parameters Parameters { get; }

   string Image { get; }

   bool Constructing { get; }
}