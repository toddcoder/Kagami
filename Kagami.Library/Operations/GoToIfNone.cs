namespace Kagami.Library.Operations;

public class GoToIfNone : GoToIfSome
{
   public GoToIfNone() => predicate = o => o.IsNone;

   public override string ToString() => "goto.if.none";
}