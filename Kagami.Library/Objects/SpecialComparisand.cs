using Core.Collections;
using Core.Numbers;

namespace Kagami.Library.Objects;

public struct SpecialComparisand(SpecialComparisandDirection direction, IObject value) : IObject
{
   private Bits32<SpecialComparisandDirection> direction = direction;
   private IObject value = value;

   public string ClassName => "SpecialComparisand";

   private string directionString()
   {
      if (direction[SpecialComparisandDirection.Less])
      {
         return direction[SpecialComparisandDirection.Equal] ? "<=" : "<";
      }
      else if (direction[SpecialComparisandDirection.Greater])
      {
         return direction[SpecialComparisandDirection.Equal] ? ">=" : ">";
      }
      else if (direction[SpecialComparisandDirection.Equal])
      {
         return direction[SpecialComparisandDirection.Not] ? "!=" : "==";
      }
      else
      {
         return "???";
      }
   }

   public string AsString => $"{directionString()} {value.AsString}";

   public string Image => $"{directionString()} {value.Image}";

   public int Hash => value.Hash;

   public bool IsEqualTo(IObject obj) => obj is SpecialComparisand other && direction == other.direction && value.IsEqualTo(other.value);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => directionString() switch
   {
      "==" => comparisand.IsEqualTo(value),
      "!=" => !comparisand.IsEqualTo(value),
      "<" => comparisand is IComparable comparable && comparable.CompareTo(value) < 0,
      "<=" => comparisand is IComparable comparableLessEqual && comparableLessEqual.CompareTo(value) <= 0,
      ">" => comparisand is IComparable comparableGreater && comparableGreater.CompareTo(value) > 0,
      ">=" => comparisand is IComparable comparableGreaterEqual && comparableGreaterEqual.CompareTo(value) >= 0,
      _ => false
   };

   public bool IsTrue => !direction[SpecialComparisandDirection.Not];

   public Guid Id { get; init; } = Guid.NewGuid();
}