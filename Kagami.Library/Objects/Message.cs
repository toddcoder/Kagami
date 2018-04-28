using System;
using Standard.Types.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public struct Message : IObject, IEquatable<Message>
   {
      string name;
      Arguments arguments;

      public Message(string name, Arguments arguments) : this()
      {
         this.name = name;
         this.arguments = arguments;
      }

      public Message(string name, params IObject[] arguments) : this()
      {
         this.name = name;
         this.arguments = new Arguments(arguments);
      }

      public Message(string name) : this()
      {
         this.name = name;
         arguments = Arguments.Empty;
      }

      public string Name => name;

      public Arguments Arguments => arguments;

      public string ClassName => "Message";

      public string AsString => $"?{name}({arguments.AsString})";

      public string Image => $"?{name}({arguments.Image})";

      public int Hash
      {
         get
         {
            unchecked
            {
               return (name?.GetHashCode() ?? 0) * 397 ^ arguments.GetHashCode();
            }
         }
      }

      public bool IsEqualTo(IObject obj) => obj is Message m && name == m.name && arguments.IsEqualTo(m.arguments);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

	   public bool IsTrue => true;

      public bool Equals(Message other) => string.Equals(name, other.name) && arguments.Equals(other.arguments);

      public override bool Equals(object obj) => obj is Message message && Equals(message);

      public override int GetHashCode() => Hash;
   }
}