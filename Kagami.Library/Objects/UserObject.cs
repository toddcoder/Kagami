﻿using Kagami.Library.Invokables;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public class UserObject : IObject
   {
      string className;
      Fields fields;
      Parameters parameters;
      int objectID;

      public UserObject(string className, Fields fields, Parameters parameters)
      {
         this.fields = fields;
         this.className = className;
         this.parameters = parameters;

         objectID = uniqueObjectID();

         setField("self", this);
         setField("id", (Int)objectID);
      }

      void setField(string fieldName, IObject value)
      {
         if (!fields.ContainsKey(fieldName))
            fields.New(fieldName, value);
         fields.Assign(fieldName, value, true).Force();
      }

      public Fields Fields => fields;

      public Parameters Parameters => parameters;

      public int ObjectID => objectID;

      public string ClassName => className;

      public string AsString => userObjectString(this);

      public string Image => userObjectString(this);

      public int Hash => objectID;

      public bool IsEqualTo(IObject obj) => isEqualTo(this, obj);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => userObjectMatch(this, comparisand, bindings);

      public bool IsTrue => Boolean.Object(fields.Length > 0).IsTrue;
   }
}