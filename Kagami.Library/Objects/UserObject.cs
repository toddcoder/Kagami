using Kagami.Library.Invokables;
using Kagami.Library.Runtime;
using Core.Collections;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class UserObject : IObject, IEquatable<UserObject>
{
   private readonly string className;
   private readonly Fields fields;
   private readonly Parameters parameters;
   private readonly int objectID;

   public UserObject(string className, Fields fields, Parameters parameters)
   {
      this.fields = fields;
      this.className = className;
      this.parameters = parameters;

      objectID = uniqueObjectID();

      setField("self", this);
      setField("id", (Int)objectID);
   }

   protected void setField(string fieldName, IObject value)
   {
      if (fields.ContainsKey(fieldName))
      {
         fields.Remove(fieldName);
      }

      fields.New(fieldName, FieldType.Assignment, value);
   }

   public Fields Fields => fields;

   public Parameters Parameters => parameters;

   public IEnumerable<IObject> ParameterValues => parameters.Select(p => fields.ContainsKey(p.Name) ? fields[p.Name] : Unassigned.Value);

   public int ObjectID => objectID;

   public string ClassName => className;

   public string AsString => userObjectString(this);

   public string Image => userObjectImage(this);

   public int Hash => HashCode.Combine(ClassName, parameters);

   public bool IsEqualTo(IObject obj) => isEqualTo(this, obj);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => userObjectMatch(this, comparisand, bindings);

   public bool IsTrue => KBoolean.BooleanObject(fields.Length > 0).IsTrue;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IObject With(IObject args)
   {
      if (args is Dictionary dictionary)
      {
         var hash = dictionary.InternalHash;
         foreach (var (key, value) in hash)
         {
            setField(key.AsString, value);
         }

         return this;
      }
      else
      {
         throw fail("Dictionary required as the argument for with");
      }
   }

   public UserObject Clone()
   {
      var newFields = fields.Clone();
      return new UserObject(className, newFields, parameters);
   }

   public bool Equals(UserObject? other) => other is not null && isEqualTo(this, other);

   public override bool Equals(object? obj) => obj is UserObject other && other.Equals(this);

   public override int GetHashCode() => Hash;

   public static bool operator ==(UserObject? left, UserObject? right) => Equals(left, right);

   public static bool operator !=(UserObject? left, UserObject? right) => !Equals(left, right);
}