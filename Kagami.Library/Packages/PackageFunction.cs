using System;
using Kagami.Library.Objects;
using Standard.Types.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Packages
{
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

      public IObject Invoke(Arguments arguments)
      {
         var message = new Message(name, arguments);
         return function(package, message);
      }

      public bool Equals(PackageFunction other)
      {
         return Equals(package, other.package) && string.Equals(name, other.name) && Equals(function, other.function);
      }

      public override bool Equals(object obj) => obj.GetType() == GetType() && Equals((PackageFunction)obj);

      public override int GetHashCode()
      {
         unchecked
         {
            var hashCode = package != null ? package.GetHashCode() : 0;
            hashCode = hashCode * 397 ^ (name != null ? name.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ (function != null ? function.GetHashCode() : 0);
            return hashCode;
         }
      }

      public IObject Invoke(IObject[] arguments) => Invoke(new Arguments(arguments));
   }
}