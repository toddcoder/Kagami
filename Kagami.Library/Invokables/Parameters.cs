using System.Collections;
using Kagami.Library.Objects;
using Core.Enumerables;

namespace Kagami.Library.Invokables;

public class Parameters : IEquatable<Parameters>, IEnumerable<Parameter>
{
   public static IEnumerable<string> NamesFromCount(int count)
   {
      return Enumerable.Range(0, count).Select(i => (char)(i + 97)).Select(c => c.ToString().get());
   }

   public static Parameters Empty => new();

   protected Parameter[] parameters;
   protected Parameter[] noCapturingParameters;

   public Parameters(params Parameter[] parameters)
   {
      this.parameters = [.. parameters.Where(p => !p.NoCapturing)];
      noCapturingParameters = [.. parameters.Where(p => p.NoCapturing)];
   }

   public Parameters() : this((string[]) [])
   {
   }

   public Parameters(int count) : this([.. Enumerable.Range(0, count).Select(i => Parameter.New(false, $"__${i}"))])
   {
   }

   public Parameters(params string[] parameterNames) : this([.. parameterNames.Select(n => Parameter.New(false, n))])
   {
   }

   public bool Equals(Parameters? other)
   {
      return other is not null && parameters.Length == other.parameters.Length &&
         parameters.Zip(other.parameters, (p1, p2) => (x: p1, y: p2)).All(t => t.x.Equals(t.y));
   }

   public override bool Equals(object? obj) => Equals((Parameters)obj!);

   public override int GetHashCode() => parameters.GetHashCode();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public int Length => parameters.Length;

   public Parameter[] GetParameters() => parameters;

   public Parameter[] GetNoCapturingParameters() => noCapturingParameters;

   public Parameter this[int index] => parameters[index];

   public IEnumerator<Parameter> GetEnumerator()
   {
      foreach (var parameter in parameters)
      {
         yield return parameter;
      }
   }

   public override string ToString() => parameters.ToString(", ");

   public Selector Selector(string name) => name.Selector([.. parameters.Select(p => p.NameForFunction)]);
}