﻿using System.Collections;
using Core.Collections;
using Core.Enumerables;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct Arguments : IObject, IEnumerable<IObject>, IEquatable<Arguments>
{
   public static Arguments Append(Arguments arguments, IObject item)
   {
      var newArguments = new IObject[arguments.Length + 1];
      Array.Copy(arguments.arguments, newArguments, arguments.Length);
      newArguments[^1] = item;

      return new Arguments(newArguments);
   }

   public static Arguments Empty => new([]);

   private readonly IObject[] arguments;
   private readonly string[] labels;

   public Arguments(params IObject[] arguments) : this()
   {
      this.arguments = arguments.Select(a => a is NameValue nv ? nv.Value : a).ToArray();
      labels = arguments.Select(a => a is NameValue nv ? nv.Name : "").ToArray();
   }

   public string ClassName => "Arguments";

   public string AsString => arguments.Select(i => i.AsString).ToString(", ");

   public string Image => arguments.Select(i => i.Image).ToString(", ");

   public int Hash => arguments.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Arguments a && compareEnumerables(arguments, a.arguments);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => arguments.Length > 0;

   public Guid Id { get; init; } = Guid.NewGuid();

   public int Length => arguments.Length;

   public IObject this[int index]
   {
      get
      {
         if (index >= arguments.Length)
         {
            throw fail("Argument missing");
         }
         else if (index < 0)
         {
            throw fail("Malformed argument request");
         }
         else
         {
            return arguments[index];
         }
      }
   }

   public Selector Selector(string name) => selector(name, labels, arguments);

   public IObject[] Value => arguments;

   public IEnumerator<IObject> GetEnumerator()
   {
      foreach (var argument in arguments)
      {
         yield return argument;
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public Arguments Pass(int count) => new(arguments.Skip(count).ToArray());

   public Arguments Prepend(IObject prefix)
   {
      var list = arguments.ToList();
      list.Insert(0, prefix);

      return new Arguments(list.ToArray());
   }

   public bool Equals(Arguments other) => Equals(arguments, other.arguments) && Equals(labels, other.labels);

   public override bool Equals(object? obj) => obj is Arguments other && Equals(other);

   public override int GetHashCode() => HashCode.Combine(arguments, labels);

   public static bool operator ==(Arguments left, Arguments right) => left.Equals(right);

   public static bool operator !=(Arguments left, Arguments right) => !left.Equals(right);
}