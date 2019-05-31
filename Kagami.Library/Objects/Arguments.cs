using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Enumerables;
using Core.Exceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public struct Arguments : IObject, IEnumerable<IObject>
   {
      public static Arguments Append(Arguments arguments, IObject item)
      {
         var newArguments = new IObject[arguments.Length + 1];
         System.Array.Copy(arguments.arguments, newArguments, arguments.Length);
         newArguments[newArguments.Length - 1] = item;

         return new Arguments(newArguments);
      }

      public static Arguments Empty => new Arguments(new IObject[] { });

      IObject[] arguments;
      string[] labels;

      public Arguments(params IObject[] arguments) : this()
      {
         this.arguments = arguments.Select(a => a is NameValue nv ? nv.Value : a).ToArray();
         labels = arguments.Select(a => a is NameValue nv ? nv.Name : "").ToArray();
      }

      public string ClassName => "Arguments";

      public string AsString => arguments.Select(i => i.AsString).Stringify();

      public string Image => arguments.Select(i => i.Image).Stringify();

      public int Hash => arguments.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Arguments a && compareEnumerables(arguments, a.arguments);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => arguments.Length > 0;

      public int Length => arguments.Length;

      public IObject this[int index]
      {
         get
         {
            if (index >= arguments.Length)
               throw "Argument missing".Throws();
            else if (index < 0)
               throw "Malformed argument request".Throws();
            else
               return arguments[index];
         }
      }

      public Selector Selector(string name) => selector(name, labels, arguments);

      public IObject[] Value => arguments;

      public IEnumerator<IObject> GetEnumerator()
      {
         foreach (var argument in arguments)
            yield return argument;
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public Arguments Pass(int count) => new Arguments(arguments.Skip(count).ToArray());

      public Arguments Prepend(IObject prefix)
      {
         var list = arguments.ToList();
         list.Insert(0, prefix);

         return new Arguments(list.ToArray());
      }
   }
}