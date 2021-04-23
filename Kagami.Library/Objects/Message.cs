using System;
using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public readonly struct Message : IObject, IEquatable<Message>
   {
      private readonly Selector selector;
      private readonly Arguments arguments;

      public Message(Selector selector, Arguments arguments) : this()
      {
         this.arguments = arguments;
         this.selector = arguments.Selector(selector.Name);
      }

      public Message(Selector selector, params IObject[] arguments) : this()
      {
         this.arguments = new Arguments(arguments);
         this.selector = this.arguments.Selector(selector.Name);
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