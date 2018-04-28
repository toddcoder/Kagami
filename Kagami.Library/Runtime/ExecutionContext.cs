namespace Kagami.Library.Runtime
{
   public class ExecutionContext
   {
      public static ExecutionContext State { get; set; } = new ExecutionContext();

/*      public Machine Machine { get; set; }

      public Module GlobalModule { get; set; }

      public Fields CurrentFields => Machine.CurrentFrame.Fields;*/
   }
}