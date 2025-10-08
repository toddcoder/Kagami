namespace Kagami.Library;

public class CompilerConfiguration
{
   public static CompilerConfiguration Empty => new();

   public bool ShowOperations { get; set; }

   public bool Tracing { get; set; }
}