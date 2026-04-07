using Core.Collections;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public struct Failure : IObject, IResult, IMonad, IBoolean
{
   public static IObject Object(string message) => new Failure(message);

   private Protocols.Protocol erroring = Protocols.Protocols.GetOrThrow("PError");
   private ProtocolWrapper wrapper;

   public Failure(Error error) : this()
   {
      Error = erroring;
      wrapper = new ProtocolWrapper(error, erroring);
   }

   public Failure(string message) : this(new Error(message, Machine.Current.CallStack))
   {
   }

   public Failure(IObject errorObject)
   {
      Error = erroring;
      wrapper = new ProtocolWrapper(errorObject, erroring);
   }

   private string message() => wrapper.SendMessage("message".get()).AsString;

   public string ClassName => "Failure";

   public string AsString => message();

   public string Image => $"f\"{message()}\"";

   public int Hash => wrapper.Hash;

   public bool IsEqualTo(IObject obj) => obj is Failure failure && wrapper.IsEqualTo(failure.wrapper.Object);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings)
   {
      return match(this, comparisand, (f1, f2) => f1.wrapper.Match(f2.wrapper.Object, bindings), bindings);
   }

   public bool IsTrue => false;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IObject Value => throw fail(message());

   public Protocols.Protocol Error { get; }

   public bool IsSuccess => false;

   public bool IsFailure => true;

   public IObject Map(Lambda lambda) => this;

   public IObject FlatMap(Lambda ifSuccess, Lambda ifFailure) => ifFailure.Invoke(wrapper.Object);

   public IObject Optional() => KNil.NilValue;

   public IObject Bind(Lambda map) => Map(map);

   public IObject Unit(IObject obj) => new Failure(wrapper.Object);

   public KBoolean CanBind => false;

   public IObject ErrorObject => wrapper;

   public KString Message => new(sendMessage(wrapper.Object, "message".get()).AsString);

   public KString CallStack => new(sendMessage(wrapper.Object, "callStack".get()).AsString);
}