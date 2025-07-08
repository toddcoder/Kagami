using Core.Collections;
using Core.Enumerables;
using Core.Strings;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public struct UserObjectPlaceholder(string name) : IObject, IEquatable<UserObjectPlaceholder>, IRuntimeArguments
{
   public string Name => name;

   public IObject[] Arguments { get; set; } = [];

   public string ClassName => "UserObjectPlaceholder";

   public string AsString => $"{name}({Arguments.ToString(", ")})";

   public string Image => AsString;

   public int Hash => HashCode.Combine(name, Arguments);

   public bool IsEqualTo(IObject obj) => obj is UserObjectPlaceholder placeholder && Equals(placeholder);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();

   public bool Equals(UserObjectPlaceholder other) => name == other.Name && Arguments.SequenceEqual(other.Arguments);

   public bool Match(UserObject userObject, Hash<string, IObject> bindings)
   {
      var userObjectClassName = userObject.ClassName;
      var _index = userObjectClassName.Find('$');
      if (_index is (true, var index))
      {
         userObjectClassName = userObjectClassName.Drop(index + 1);
      }

      if (name == userObjectClassName && Arguments.Length == userObject.Parameters.Length)
      {
         var fields = userObject.Fields;
         for (var i = 0; i < Arguments.Length; i++)
         {
            var argument = Arguments[i];
            var parameterName = userObject.Parameters[i].Name;
            var _value = fields.GetFieldValue(parameterName);
            if (_value is (true, var value))
            {
               if (argument is Placeholder placeholder)
               {
                  bindings[placeholder.Name] = value;
               }
               else if (!argument.Match(value, bindings))
               {
                  return false;
               }
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

   public void SetArguments(IObject[] arguments)
   {
      Arguments = arguments;
   }
}