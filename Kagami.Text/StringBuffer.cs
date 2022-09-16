using System.Text;
using Kagami.Library.Objects;
using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Text
{
   public class StringBuffer : IObject
   {
      protected StringBuilder builder;

      public StringBuffer() => builder = new StringBuilder();

      public StringBuffer(string initialValue) => builder = new StringBuilder(initialValue);

      public string ClassName => "StringBuffer";

      public string AsString => builder.ToString();

      public string Image => AsString;

      public int Hash => builder.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is StringBuffer sb && AsString == sb.AsString;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => builder.Length > 0;

      public Char this[int index]
      {
         get => builder[index];
         set => builder[index] = value.Value;
      }

      public StringBuffer Append(IObject value)
      {
         builder.Append(value.AsString);
         return this;
      }

      public Int Length => builder.Length;

      public StringBuffer Clear()
      {
         builder.Clear();
         return this;
      }
   }
}