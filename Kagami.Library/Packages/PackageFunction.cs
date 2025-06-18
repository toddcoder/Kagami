using Kagami.Library.Objects;
using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Packages;

public class PackageFunction : IObject, IEquatable<PackageFunction>, IMayInvoke
{
   protected Package package;
   protected string name;
   protected Func<IObject, Message, IObject> function;

   public PackageFunction(Package package, string name, Func<IObject, Message, IObject> function)
   {
      this.package = package;
      this.name = name;
      this.function = function;
   }

   public string ClassName => "PackageFunction";

   public string AsString => $"{package.ClassName}.{name}()";

   public string Image => AsString;

   public int Hash => name.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is PackageFunction packageFunction && AsString == packageFunction.AsString;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IObject Invoke(Arguments arguments)
   {
      var message = new Message(name, arguments);
      return function(package, message);
   }

   public bool Equals(PackageFunction? other)
   {
      return other is not null && Equals(package, other.package) && string.Equals(name, other.name) && Equals(function, other.function);
   }

   public override bool Equals(object? obj) => obj is not null && obj.GetType() == GetType() && Equals((PackageFunction)obj);

   public override int GetHashCode() => HashCode.Combine(package, name, function);

   public IObject Invoke(IObject[] arguments) => Invoke(new Arguments(arguments));
}