namespace Kagami.Library.Operations;

public class GoToIfFalse : GoToIfTrue
{
   public GoToIfFalse()
   {
      predicate = b => !b.IsTrue;
   }

   public override string ToString() => $"goto.if.false({address})";
}