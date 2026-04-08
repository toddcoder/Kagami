using Core.Monads;
using Kagami.Library.Classes;
using Kagami.Library.Protocols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class ProtocolConstraint(string protocolName) : TypeConstraint([])
{
   protected Lazy<Protocol> protocol = new(() => Protocols.Protocols.GetOrThrow(protocolName));

   public Protocol Protocol => protocol.Value;

   public override int Hash => protocol.Value.Name.GetHashCode();

   public override bool IsEqualTo(IObject obj)
   {
      return obj is ProtocolConstraint protocolConstraint && protocol.Value.Name == protocolConstraint.protocol.Value.Name;
   }

   public override bool Matches(BaseClass baseClass) => protocol.Value.Supports(baseClass);

   public override bool Matches(UserClass userClass) => Matches((BaseClass)userClass);

   public override bool IsEquivalentTo(TypeConstraint typeConstraint) => typeConstraint switch
   {
      null => false,
      ProtocolConstraint pc => protocol.Value.Name == pc.protocol.Value.Name,
      _ => typeConstraint.Comparisands.Any(c => protocol.Value.Supports(c))
   };

   public override IEnumerator<TypeConstraint> GetEnumerator()
   {
      yield return new ProtocolConstraint(protocolName);
   }

   public override string AsString => protocolName;

   public override Maybe<IObject> ConvertToMonad(IObject value)
   {
      return protocolName switch
      {
         "POptional" => value switch
         {
            Some or KNil => value.Some(),
            Success success => Some.Object(success.Value).Some(),
            Failure or Error => KNil.NilValue.Some(),
            _ => Some.Object(value).Some()
         },
         "PResult" => value switch
         {
            Success or Failure => value.Some(),
            Error error => new Failure(error),
            Some some => Success.Object(some.Value).Some(),
            KNil => Failure.Object("No value provided").Some(),
            _ when supportsErroring() => new Failure(value),
            _ => Success.Object(value).Some()
         },
         _ => nil
      };

      bool supportsErroring()
      {
         var erroring = Protocols.Protocols.GetOrThrow("PError");
         return erroring.Supports(value);
      }
   }
}