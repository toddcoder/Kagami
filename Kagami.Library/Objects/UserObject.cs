using Kagami.Library.Invokables;
using Kagami.Library.Runtime;
using Core.Collections;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class UserObject : IObject
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

   public Guid Id { get; init; } = Guid.NewGuid();

   public IObject With(IObject args)
   {
      Hash<string, IObject> newFields = [];
      switch (args)
      {
         case KTuple tuple:
         {
            var iterator = tuple.GetIterator(false);
            var list = iterator.List();
            foreach (var nameValue in list.Cast<NameValue>())
            {
               newFields[nameValue.Name] = nameValue.Value;
            }

            break;
         }
         case KArray array:
         {
            var iterator = array.GetIterator(false);
            var list = iterator.List();
            foreach (var nameValue in list.Cast<NameValue>())
            {
               newFields[nameValue.Name] = nameValue.Value;
            }

            break;
         }
         case Dictionary dictionary:
         {
            var hash = dictionary.InternalHash;
            foreach (var (key, value) in hash)
            {
               var name = key.AsString;
               newFields[name] = value;
            }

            break;
         }
         case NameValue nameValue:
            newFields[nameValue.Name] = nameValue.Value;
            break;
      }

      var selector = parameters.Selector(className);

      List<IObject> arguments = [];
      foreach (var parameter in parameters)
      {
         if (newFields.Maybe[parameter.Name] is (true, var value))
         {
            arguments.Add(value);
         }
         else
         {
            var _field = fields.Find(parameter.Name, true);
            if (_field is (true, var field))
            {
               arguments.Add(field.Value);
            }
            else if (_field.Exception is (true, var exception))
            {
               throw exception;
            }
            else
            {
               throw fail($"Couldn't find field {parameter.Name}");
            }
         }
      }

      var message = new Message(selector, [.. arguments]);
      return createObject(selector, message);
   }
}