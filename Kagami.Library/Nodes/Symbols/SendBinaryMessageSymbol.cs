﻿using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Core.Strings;

namespace Kagami.Library.Nodes.Symbols;

public class SendBinaryMessageSymbol : Symbol
{
   protected Selector selector;
   protected Precedence precedence;
   protected bool swap;
   protected string label;

   public SendBinaryMessageSymbol(Selector selector, Precedence precedence, bool swap = false, string label = "")
   {
      this.selector = selector;
      this.precedence = precedence;
      this.swap = swap;
      this.label = label;
   }

   public override void Generate(OperationsBuilder builder)
   {
      if (swap)
      {
         builder.Swap();
      }

      if (label.IsNotEmpty())
      {
         builder.ArgumentLabel(label);
      }

      builder.SendMessage(selector, 1);
   }

   public override Precedence Precedence => precedence;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => selector.Image;
}