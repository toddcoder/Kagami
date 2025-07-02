using Core.Collections;
using Core.Enumerables;
using Core.Strings;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct UserObjectPlaceholder(string name, string[] parameters) : IObject, IEquatable<UserObjectPlaceholder>
{
   public string Name => name;

   public string[] Parameters => parameters;

   public string ClassName => "UserObjectPlaceholder";

   public string AsString => $"{name}({parameters.ToString(", ")})";

   public string Image => AsString;

   public int Hash => HashCode.Combine(name, parameters);

   public bool IsEqualTo(IObject obj) => obj is UserObjectPlaceholder placeholder && Equals(placeholder);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();

   public bool Equals(UserObjectPlaceholder other) => name == other.Name && parameters.SequenceEqual(other.Parameters);

   public bool Match(UserObject userObject, Hash<string, IObject> bindings)
   {
      var userObjectClassName = userObject.ClassName;
      var _index = userObjectClassName.Find('$');
      if (_index is (true, var index))
      {
         userObjectClassName = userObjectClassName.Drop(index + 1);
      }
      if (name == userObjectClassName && parameters.Length == userObject.Parameters.Length)
      {
         var fields = userObject.Fields;
         for (var i = 0; i < parameters.Length; i++)
         {
            var parameter = parameters[i];
            var parameterName = userObject.Parameters[i].Name;
            var _value = fields.GetFieldValue(parameterName);
            if (_value is (true, var value))
            {
               bindings[$"-{parameter}"] = value;
            }
            else
            {
               return false;
            }
         }

         return true;
      }
      else
      {
         return false;
      }
   }
}