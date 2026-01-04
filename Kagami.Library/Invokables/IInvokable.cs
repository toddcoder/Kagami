using Core.Monads;
using Kagami.Library.Objects;

namespace Kagami.Library.Invokables;

public interface IInvokable
{
   int Index { get; set; }

   int Address { get; set; }

   Parameters Parameters { get; }

   string Image { get; }

   bool Constructing { get; }

   public bool RequiresFunctionFrame { get; }

   public Maybe<Class> Class { get; set; }
}