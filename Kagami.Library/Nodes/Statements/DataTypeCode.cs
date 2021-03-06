﻿using Kagami.Library.Objects;
using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements
{
   public class DataTypeCode : Statement
   {
      protected string className;
      protected string name;
      protected IObject[] comparisands;
      protected IObject ordinal;

      public DataTypeCode(string className, string name, IObject[] comparisands, IObject ordinal)
      {
         this.className = className;
         this.name = name;
         this.comparisands = comparisands;
         this.ordinal = ordinal;
      }

      public override void Generate(OperationsBuilder builder)
      {
         var failedMatchLabel = newLabel("failed-match");
         var i = 0;

         foreach (var comparisand in comparisands)
         {
            var parameterName = $"__${i++}";
            builder.GetField(parameterName);
            builder.PushObject(comparisand);
            builder.Match();
            builder.GoToIfFalse(failedMatchLabel);
         }

         builder.PushString(className);
         builder.PushString(name);
         for (var j = 0; j < comparisands.Length; j++)
         {
            var parameterName = $"__${j}";
            builder.GetField(parameterName);
         }

         builder.ToArguments(comparisands.Length);
         builder.PushObject(ordinal);
         builder.NewDataComparisand();
         builder.Return(true);

         builder.Label(failedMatchLabel);
         builder.PushObject(Unmatched.Value);
         builder.Return(true);
      }
   }
}