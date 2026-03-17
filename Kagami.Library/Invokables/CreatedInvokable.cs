using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Invokables;

public struct CreatedInvokable(int address, string image) : IInvokable
{
   public int Index { get; set; } = -1;

   public int Address { get; set; } = address;

   public Parameters Parameters => Parameters.Empty;

   public string Image => image;

   public bool Constructing => false;

   public bool RequiresFunctionFrame => true;

   public Maybe<Class> Class { get; set; } = nil;
}