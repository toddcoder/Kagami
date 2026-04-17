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
   AddSubtract = 8,
   Shift = 9,
   Boolean = 10,
   Range = 11,
   And = 12,
   Or = 13,
   Format = 14,
   Concatenate = 15,
   ChainedOperator = 16,
   KeyValue = 17,
   Comma = 18,
   Pipeline = 19
}