﻿using System.Linq;
using Standard.Types.Enumerables;

namespace Kagami.Library.Invokables
{
   public class ConstructorInvokable : IInvokable
   {
      public ConstructorInvokable(string className, Parameters parameters)
      {
         ClassName = className;
         Parameters = parameters;
      }

      string ClassName { get; }

      public int Index { get; set; }

      public int Address { get; set; }

      public Parameters Parameters { get; }

      public string Image => $"{ClassName}({Parameters.Select(p => p.Name).Listify()})";

      public bool Constructing => true;
   }
}