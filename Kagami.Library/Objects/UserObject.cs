using Kagami.Library.Invokables;
using Kagami.Library.Runtime;
using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class UserObject : IObject
{
   private readonly string className;
   private readonly Fields fields;
   private readonly Parameters parameters;
   private readonly int objectID = uniqueObjectID();

   public UserObject(string className, Fields fields, Parameters parameters)
   {
      this.fields = fields;
      this.className = className;
      this.parameters = parameters;

      setField("self", this);
      setField("id", (Int)objectID);
   }

   protected void setField(string fieldName, IObject value)
   {
      if (fields.ContainsKey(fieldName))
      {
         fields.Remove(fieldName);
      }

      fields.New(fieldName, value);
   }

   public Fields Fields => fields;

   public Parameters Parameters => parameters;

   public int ObjectID => objectID;

   public string ClassName => className;

   public string AsString => userObjectString(this);

   public string Image => userObjectImage(this);

   public int Hash => objectID;

   public bool IsEqualTo(IObject obj) => isEqualTo(this, obj);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => userObjectMatch(this, comparisand, bindings);

   public bool IsTrue => KBoolean.BooleanObject(fields.Length > 0).IsTrue;
}