﻿using Kagami.Library.Classes;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public class DataType : IObject
   {
      string className;
      Hash<string, (IObject[], IObject)> comparisands;

      public DataType(string className, Hash<string, (IObject[], IObject)> comparisands)
      {
         this.className = className;
         this.comparisands = comparisands;
         var cls = (DataTypeClass)classOf(this);
         cls.RegisterDataType(this);
      }

      public string ClassName => className;

      public string AsString => comparisands.KeyArray().Listify(" | ");

      public string Image => AsString;

      public int Hash => comparisands.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is DataType dataType && className == dataType.className;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => true;

      public IObject GetDataComparisand(string name, IObject[] arguments)
      {
         var self = this;
         return comparisands.FlatMap(name, c =>
         {
            (var data, var ordinal) = c;
            if (checkParameters(data, arguments))
               return new DataComparisand(self.className, name, data, ordinal);
            else
               return Unmatched.Value;
         }, () => Unmatched.Value);
      }

      public bool ContainsDataComparisand(string name) => comparisands.ContainsKey(name);

      static bool checkParameters(IObject[] comparisands, IObject[] arguments)
      {
         var length = comparisands.Length;
         if (arguments.Length != length)
            return false;
         else
         {
            var bindings = new Hash<string, IObject>();
            for (var i = 0; i < length; i++)
            {
               var value = arguments[i];
               var comparisand = comparisands[i];
               if (!value.Match(comparisand, bindings))
                  return false;
            }

            Machine.Current.CurrentFrame.Fields.SetBindings(bindings, true, false);
            return true;
         }
      }
   }
}