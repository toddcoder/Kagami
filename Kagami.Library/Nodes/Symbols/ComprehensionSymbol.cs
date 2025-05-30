﻿using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class ComprehensionSymbol : Symbol
{
   protected Block block;
   protected string image;

   public ComprehensionSymbol(Block block, string image)
   {
      this.block = block;
      this.image = image;
   }

   public override void Generate(OperationsBuilder builder)
   {
      var functionName = $"__$comprehension{id()}";
      var yieldingInvokable = new YieldingInvokable(functionName, Parameters.Empty, image);
      var _index = builder.RegisterInvokable(yieldingInvokable, block, true);
      if (_index)
      {
         builder.PushObject(yieldingInvokable);
      }
      else
      {
         throw _index.Exception;
      }
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => image;
}