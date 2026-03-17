using Core.Collections;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Packages;

public struct Out() : IObject
{
   public string ClassName => "Out";

   public string AsString => "Out";

   public string Image => "Out";

   public int Hash => HashCode.Combine(ClassName);

   public bool IsEqualTo(IObject obj) => obj is Out;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IObject Append(IObject item)
   {
      Machine.Current.Context.Print(item.AsString);
      return this;
   }

   public IObject AppendLine(IObject item)
   {
      Machine.Current.Context.PrintLine(item.AsString);
      return this;
   }
}