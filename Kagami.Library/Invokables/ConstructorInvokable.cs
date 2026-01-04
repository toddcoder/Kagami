using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Invokables;

public class ConstructorInvokable : IInvokable
{
   public ConstructorInvokable(string className, Parameters parameters)
   {
      ClassName = className;
      Parameters = parameters;
      Class = new Class(className);
   }

   public string ClassName { get; }

   public int Index { get; set; }

   public int Address { get; set; }

   public Parameters Parameters { get; }

   public string Image => $"{ClassName}({Parameters.Select(p => p.Name).ToString(", ")})";

   public bool Constructing => true;

   public bool RequiresFunctionFrame => true;

   public Maybe<Class> Class { get; set; } = nil;
}