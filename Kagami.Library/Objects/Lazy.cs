using Kagami.Library.Invokables;
using Kagami.Library.Runtime;
using Core.Collections;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class Lazy : IObject
{
   protected IInvokable invokable;
   protected string image;

   public Lazy(IInvokable invokable, string image)
   {
      this.invokable = invokable;
      this.image = image;
   }

   public IObject Value => Machine.Current.Value.Invoke(invokable, Arguments.Empty, nil).Force();

   public string ClassName => "Lazy";

   public string AsString => image;

   public string Image => image;

   public int Hash => image.GetHashCode();

   public bool IsEqualTo(IObject obj) => image == obj.Image;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings)
   {
      if (comparisand is Placeholder placeholder)
      {
         bindings[placeholder.Name] = this;
         return true;
      }
      else
      {
         return false;
      }
   }

   public bool IsTrue => false;

   public Guid Id { get; init; } = Guid.NewGuid();
}