using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Standard.Types.Enumerables;
using Standard.Types.Strings;

namespace Kagami.Library.Invokables
{
   public class Parameters : IEquatable<Parameters>, IEnumerable<Parameter>
   {
      public static IEnumerable<string> NamesFromCount(int count)
      {
         return Enumerable.Range(0, count).Select(i => (char)(i + 97)).Select(c => c.ToString().get());
      }

      public static Parameters Empty => new Parameters();

      Parameter[] parameters;

      public Parameters(Parameter[] parameters) => this.parameters = parameters;

      public Parameters() : this(new Parameter[0]) { }

      public Parameters(int count) :
         this(Enumerable.Range(0, count).Select(i => Parameter.New(false, i.ToString().get())).ToArray()) { }

      public bool Equals(Parameters other)
      {
         return parameters.Length == other.parameters.Length &&
            parameters.Zip(other.parameters, (p1, p2) => (x: p1, y: p2)).All(t => t.x.Equals(t.y));
      }

      public override bool Equals(object obj) => Equals((Parameters)obj);

      public override int GetHashCode() => parameters.GetHashCode();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public int Length => parameters.Length;

      public Parameter[] GetParameters() => parameters;

      public Parameter this[int index] => parameters[index];

      public IEnumerator<Parameter> GetEnumerator()
      {
         foreach (var parameter in parameters)
            yield return parameter;
      }

      public override string ToString() => parameters.Listify();

      public string FullFunctionName(string name)
      {
         if (parameters.Any(p => p.Label.IsNotEmpty()))
            return name.Function(parameters.Select(p => p.NameForFunction).ToArray());
         else
            return name;
      }
   }
}