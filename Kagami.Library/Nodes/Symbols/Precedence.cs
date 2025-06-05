namespace Kagami.Library.Nodes.Symbols;

public enum Precedence
{
   Value = 0,
   SendMessage = 1,
   PrefixOperator = 2,
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
   ChainedOperator = 20,
   KeyValue = 21,
   Comma = 22
}