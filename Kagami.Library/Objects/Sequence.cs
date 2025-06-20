﻿using Core.Collections;
using Core.Enumerables;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class Sequence : IObject
{
   protected List<IObject> list;

   public Sequence(IObject x, IObject y)
   {
      list = [x, y];
      ExpandInTuple = true;
   }

   public Sequence(IEnumerable<IObject> objects)
   {
      list = [..objects];
      ExpandInTuple = true;
   }

   public List<IObject> List => list;

   public string ClassName => "Sequence";

   public string AsString => list.Select(i => i.AsString).ToString(" ");

   public string Image => list.Select(i => i.Image).ToString(", ");

   public int Hash => list.GetHashCode();

   public bool IsEqualTo(IObject obj)
   {
      return obj is Sequence container && list.Count == container.list.Count && list.All(i => list.Contains(i));
   }

   public bool Match(IObject comparisand, Hash<string, IObject> bindings)
   {
      return matchSingle(this, comparisand, (container, o) => container.In(o), bindings);
   }

   public bool IsTrue => list.Count > 0;

   public Guid Id { get; init; } = Guid.NewGuid();

   public void Add(IObject item) => list.Add(item);

   public bool In(IObject obj) => list.Contains(obj);

   public bool ExpandInTuple { get; set; }
}