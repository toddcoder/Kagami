using System.Linq;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public struct Arguments : IObject
   {
      public static Arguments Append(Arguments arguments, IObject item)
      {
         var newArguments = new IObject[arguments.Length + 1];
         System.Array.Copy(arguments.arguments, newArguments, arguments.Length);
         newArguments[newArguments.Length - 1] = item;

         return new Arguments(newArguments);
      }

      public static Arguments Empty = new Arguments(new IObject[] { });

      IObject[] arguments;
      Hash<string, int> labels;
      Hash<int, string> indexes;

      public Arguments(params IObject[] arguments) : this()
      {
         this.arguments = arguments;
         labels = new Hash<string, int>();
         indexes = new Hash<int, string>();

         for (var i = 0; i < arguments.Length; i++)
            if (arguments[i] is NameValue nv)
            {
               this.arguments[i] = nv.Value;
               labels[nv.Name] = i;
               indexes[i] = nv.Name;
            }
      }

      public string ClassName => "Arguments";

      public string AsString => arguments.Select(i => i.AsString).Listify();

      public string Image => arguments.Select(i => i.Image).Listify();

      public int Hash => arguments.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Arguments a && compareEnumerables(arguments, a.arguments);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => arguments.Length > 0;

      public int Length => arguments.Length;

      public IObject this[int index] => arguments[index];

      public string FullFunctionName(string name) => name.Function(labels.KeyArray());

      public IObject[] Value => arguments;
   }
}