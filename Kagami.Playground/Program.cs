namespace Kagami.Playground;

internal static class Program
{
   [STAThread]
   public static void Main()
   {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Playground());
   }
}