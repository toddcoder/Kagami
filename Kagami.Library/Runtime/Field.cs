using Kagami.Library.Objects;

namespace Kagami.Library.Runtime
{
   public class Field
   {
      public IObject Value { get; set; } = Unassigned.Value;

      public bool Mutable { get; set; }

      public bool Visible { get; set; } = true;

      public Field Clone() => new Field
      {
         Value = Value,
         Mutable = Mutable,
         Visible = Visible
      };
   }
}