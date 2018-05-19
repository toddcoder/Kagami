namespace Kagami.Library.Runtime
{
   public class Breakpoint
   {
      BreakpointType type;

      public Breakpoint(int index, int length)
      {
         Index = index;
         Length = length;
         type = BreakpointType.Soft;
      }

      public int Index { get; }

      public int Length { get; }

      public BreakpointType Type => type;

      public void Harden() => type = BreakpointType.Hard;

      public void Soften() => type = BreakpointType.Soft;
   }
}