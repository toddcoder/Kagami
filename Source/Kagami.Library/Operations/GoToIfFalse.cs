namespace Kagami.Library.Operations
{
   public class GoToIfFalse : GoToIfTrue
   {
      public GoToIfFalse() => predicate = b => !b.Value;

      public override string ToString() => $"goto.if.false({address})";
   }
}