using Kagami.Library.Invokables;

namespace Kagami.Library.Objects
{
   public class RuntimeInvokable : IInvokable
   {
      Parameters parameters;

      public RuntimeInvokable(int parameterCount, string image)
      {
         parameters = new Parameters(parameterCount);
         Image = image;
      }

      public int Index { get; set; } = -1;

      public int Address { get; set; } = -1;

      public Parameters Parameters => parameters;

      public string Image { get; }

      public bool Constructing => false;
   }
}