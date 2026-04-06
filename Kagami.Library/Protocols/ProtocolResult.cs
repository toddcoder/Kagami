using Kagami.Library.Objects;

namespace Kagami.Library.Protocols;

public abstract record ProtocolResult
{
   public sealed record Found(Protocol Protocol) : ProtocolResult;

   public sealed record NotFound : ProtocolResult;

   public sealed record Missing(IEnumerable<Selector> Selectors) : ProtocolResult;

   public sealed record Error(Exception Exception) : ProtocolResult;
}