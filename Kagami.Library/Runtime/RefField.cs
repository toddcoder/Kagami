using Kagami.Library.Objects;

namespace Kagami.Library.Runtime;

public class RefField(Field originalField) : Field
{
   public override IObject Value
   {
      get => base.Value;
      set
      {
         base.Value = value;
         originalField.Value = value;
      }
   }
}