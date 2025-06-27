namespace Kagami.Library.Operations;

[Flags]
public enum MonadType
{
   Some = 1,
   None = 2,
   Success = 4,
   Failure = 8,
}