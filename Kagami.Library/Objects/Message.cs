using System;
using Standard.Types.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public struct Message : IObject, IEquatable<Message>
   {
      Selector selector;
      Arguments arguments;

      public Message(Selector selector, Arguments arguments) : this()
      {
         this.selector = selector;
         this.arguments = arguments;
      }

      public Message(Selector selector, params IObject[] arguments) : this()
      {
         this.selector = selector;
         this.arguments = new Arguments(arguments);
      }

      public Message(Selector selector) : this()
      {
         this.selector = selector;
         arguments = Arguments.Empty;
      }

      public Selector Selector => selector;

      public Arguments Arguments => arguments;

      public string ClassName => "Message";

      public string AsString => $"?{selector.Image}({arguments.AsString})";

      public string Image => $"?{selector.Image}({arguments.Image})";

      public int Hash
      {
         get
         {
            unchecked
            {
               return selector.Hash * 397 ^ arguments.GetHashCode();
            }
         }
      }

      public bool IsEqualTo(IObject obj) => obj is Message m && selector.IsEqualTo(m.Selector) && arguments.IsEqualTo(m.arguments);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

	   public bool IsTrue => true;

      public bool Equals(Message other) => selector.IsEqualTo(other.selector) && arguments.Equals(other.arguments);

      public override bool Equals(object obj) => obj is Message message && Equals(message);

      public override int GetHashCode() => Hash;
   }
}