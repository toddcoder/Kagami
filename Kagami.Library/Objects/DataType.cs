using System.Linq;
using Kagami.Library.Classes;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Enumerables;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class DataType : IObject
{
   protected string className;
   protected Hash<string, (IObject[], IObject)> comparisands;

   public DataType(string className, Hash<string, (IObject[], IObject)> comparisands)
   {
      this.className = className;
      this.comparisands = comparisands;

      var cls = (DataTypeClass)classOf(this);
      cls.RegisterDataType(this);
   }

   public string ClassName => className;

   public string AsString => comparisands.KeyArray().ToString(" | ");

   public string Image => AsString;

   public int Hash => comparisands.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is DataType dataType && className == dataType.className;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => true;

   public IObject GetDataComparisand(string name, IObject[] arguments)
   {
      var self = this;
      return comparisands.Maybe[name].Map(c =>
      {
         var (data, ordinal) = c;
         return checkParameters(data, arguments) ? new DataComparisand(self.className, name, data, ordinal) : Unmatched.Value;
      }) | (() => Unmatched.Value);
   }

   public bool ContainsDataComparisand(string name) => comparisands.ContainsKey(name);

   protected static bool checkParameters(IObject[] comparisands, IObject[] arguments)
   {
      var length = comparisands.Length;
      if (arguments.Length != length)
      {
         return false;
      }
      else
      {
         var bindings = new Hash<string, IObject>();
         for (var i = 0; i < length; i++)
         {
            var value = arguments[i];
            var comparisand = comparisands[i];
            if (!value.Match(comparisand, bindings))
            {
               return false;
            }
         }

         Machine.Current.CurrentFrame.Fields.SetBindings(bindings);
         return true;
      }
   }

   public KArray Values
   {
      get
      {
         var values = comparisands
            .Where(i => i.Value.Item1.Length == 0)
            .OrderBy(i => i.Value.Item2)
            .Select(i => GetDataComparisand(i.Key, i.Value.Item1));
         return new KArray(values);
      }
   }
}