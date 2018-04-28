namespace Kagami.Library.Operations
{
   public class GoToIfNil : GoToIfSome
   {
      public GoToIfNil() => predicate = o => o.IsNil;

      public override string ToString() => "goto.if.nil";
   }
}