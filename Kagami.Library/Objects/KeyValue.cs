﻿using Standard.Types.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public struct KeyValue : IObject, IKeyValue
   {
      IObject key;
      IObject value;

      public KeyValue(IObject key, IObject value) : this()
      {
         this.key = key;
         this.value = value;
      }

      public string ClassName => "KeyValue";

      public string AsString => $"{key.AsString} => {value.AsString}";

      public string Image => $"{key.Image} => {value.Image}";

      public int Hash => (key.Hash + value.Hash).GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is KeyValue kv && key.IsEqualTo(kv.key) && value.IsEqualTo(kv.value);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => value.IsTrue;

      public IObject Key => key;

      public IObject Value => value;

      public bool ExpandInTuple => true;
   }
}