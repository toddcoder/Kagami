using Core.Collections;
using Kagami.Library.Protocols;

namespace Kagami.Library.Objects;

public readonly struct ProtocolWrapper(IObject obj, Protocol protocol) : IObject
{
   public IObject Object => obj;

   public string ClassName => "ProtocolWrapper";

   public string AsString => obj.AsString;

   public string Image => obj.Image;

   public int Hash => obj.Hash;

   public bool IsEqualTo(IObject other) => obj.IsEqualTo(other);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => obj.Match(comparisand, bindings);

   public bool IsTrue => obj.IsTrue;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IObject SendMessage(Selector selector, params IObject[] arguments) => protocol.SendMessage(obj, selector, arguments);
}