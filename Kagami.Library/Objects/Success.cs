using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct Success : IObject, IResult, IMonad, IBoolean
{
   public static IObject Object(IObject value, TypeConstraint typeConstraint) => value is Failure ? value : new Success(value, typeConstraint);

   public Success(IObject value, TypeConstraint typeConstraint) : this()
   {
      Value = value;
      TypeConstraint = typeConstraint;
   }

   public string ClassName => "Success";

   public string AsString => $"!{Value.AsString}";

   public string Image => $"!{Value.Image}";

   public int Hash => Value.Hash;

   public bool IsEqualTo(IObject obj) => obj is Success success && Value.IsEqualTo(success.Value) && TypeConstraint.IsEqualTo(success.TypeConstraint);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings)
   {
      return match(this, comparisand, (s1, s2) => s1.Value.Match(s2.Value, bindings) && s1.TypeConstraint.IsEqualTo(s2.TypeConstraint), bindings);
   }

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IObject Value { get; }

   public TypeConstraint TypeConstraint { get; }

   public Error Error => new("No error!");

   public bool IsSuccess => true;

   public bool IsFailure => false;

   public IObject Map(Lambda lambda)
   {
      var result = lambda.Invoke(Value);
      return result switch
      {
         Some some => new Success(some.Value, new TypeConstraint([classOf(result)])),
         KNil => new Failure("Nil value"),
         Success success => success,
         Failure failure => failure,
         _ => new Success(result, new TypeConstraint([classOf(result)]))
      };
   }

   public IObject FlatMap(Lambda ifSuccess, Lambda ifFailure) => ifSuccess.Invoke(Value);

   public IObject Optional() => new Some(Value, TypeConstraint);

   public IObject Bind(Lambda map) => Map(map);

   public IObject Unit(IObject obj) => new Success(obj, new TypeConstraint([classOf(obj)]));

   public KBoolean CanBind => true;
}