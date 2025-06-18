using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct Success : IObject, IResult, IMonad, IBoolean
{
   public static IObject Object(IObject value) => new Success(value);

   public Success(IObject value) : this() => Value = value;

   public string ClassName => "Success";

   public string AsString => $"{Value.AsString}!";

   public string Image => $"{Value.Image}!";

   public int Hash => Value.Hash;

   public bool IsEqualTo(IObject obj) => obj is Success success && Value.IsEqualTo(success.Value);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings)
   {
      return match(this, comparisand, (s1, s2) => s1.Value.Match(s2.Value, bindings), bindings);
   }

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IObject Value { get; }

   public Error Error => new("No error!");

   public bool IsSuccess => true;

   public bool IsFailure => false;

   public IObject Map(Lambda lambda)
   {
      var result = lambda.Invoke(Value);
      return result switch
      {
         Success success => success,
         Failure failure => failure,
         _ => new Success(result)
      };
   }

   public IObject FlatMap(Lambda ifSuccess, Lambda ifFailure) => ifSuccess.Invoke(Value);

   public IObject Bind(Lambda map) => Map(map);

   public IObject Unit(IObject obj) => new Success(obj);

   public KBoolean CanBind => true;
}