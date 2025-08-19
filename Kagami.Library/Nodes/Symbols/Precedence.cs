namespace Kagami.Library.Nodes.Symbols;

public enum Precedence
{
   Value = 0,
   TightPrefixOperator = 1,
   SendMessage = 2,
   PrefixOperator = 3,
   PostfixOperator = 4,
   Raise = 5,
   MultiplyDivide = 6,
   Range = 7,
   AddSubtract = 8,
   Shift = 9,
   Boolean = 10,
   And = 11,
   Or = 12,
   Format = 13,
   Concatenate = 14,
   ChainedOperator = 15,
   KeyValue = 16,
   Comma = 17,
   Pipeline = 18
}