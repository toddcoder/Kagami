using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public struct DateIncrement(Date date, int amount) : IObject
{
   public Date Date => date;

   public int Amount => amount;

   public string ClassName => "DateIncrement";

   public string AsString => $"{date.AsString} << {amount}";

   public string Image => $"{date.Image} << {amount}";

   public int Hash => HashCode.Combine(date.Hash, amount);

   public bool IsEqualTo(IObject obj) => obj is DateIncrement other && date.IsEqualTo(other.Date) && amount == other.Amount;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => amount != 0;

   public Guid Id { get; init; } = Guid.NewGuid();

   public Date Microsecond() => new(date.Value.AddMilliseconds(amount / 1000.0));

   public Date Millisecond() => new(date.Value.AddMilliseconds(amount));

   public Date Second() => new(date.Value.AddSeconds(amount));

   public Date Minute() => new(date.Value.AddMinutes(amount));

   public Date Hour() => new(date.Value.AddHours(amount));

   public Date Day() => new(date.Value.AddDays(amount));

   public Date Month() => new(date.Value.AddMonths(amount));

   public Date Year() => new(date.Value.AddYears(amount));
}