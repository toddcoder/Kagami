namespace Kagami.Library.Nodes.Symbols
{
   public enum Precedence
   {
      Value = 0,
      Indexer = 1,
      SendMessage = 1,
      PrefixOperator = 2,
      Invoke = 3,
      KeyValue = 4,
      PostfixOperator = 5,
      Raise = 6,
      MultiplyDivide = 7,
      Range = 8,
      AddSubtract = 9,
      Shift = 10,
      Boolean = 11,
      BitAnd = 12,
      BitXOr = 13,
      BitOr = 14,
      And = 15,
      Or = 16,
      Format = 17,
      Concatenate = 18,
      //Range = 19,
      ChainedOperator = 20,
      Comma = 21
   }
}