using Kagami.Library.Runtime;
using Core.Collections;

namespace Kagami.Library.Objects;

public class Reference : IObject
{
   protected Field field;

   public Reference(Field @field)
   {
      this.field = @field;
   }

   public Field Field => @field;

   public string ClassName => "Reference";

   public string AsString => @field.Value.AsString;

   public string Image => $"ref {@field.Value.Image}";

   public int Hash => @field.Value.Hash;

   public bool IsEqualTo(IObject obj)
   {
      switch (obj)
      {
         case Reference r:
            return field.Value.IsEqualTo(r.field.Value);
         default:
            return field.Value.IsEqualTo(obj);
      }
   }

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => field.Value.Match(comparisand, bindings);

   public bool IsTrue => @field.Value.IsTrue;
}