namespace Kagami.Playground;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
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