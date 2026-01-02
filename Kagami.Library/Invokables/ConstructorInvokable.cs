using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Objects;

namespace Kagami.Library.Invokables;

public class ConstructorInvokable : IInvokable
{
   public ConstructorInvokable(string className, Parameters parameters)
   {
      ClassName = className;
      Parameters = parameters;
   }

   public string ClassName { get; }

   public int Index { get; set; }

   public int Address { get; set; }

   public Parameters Parameters { get; }

   public string Image => $"{ClassName}({Parameters.Select(p => p.Name).ToString(", ")})";

   public bool Constructing => true;

   public bool RequiresFunctionFrame => true;

   public Maybe<Class> Class => new Class(ClassName);
}