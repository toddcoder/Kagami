using Kagami.Library.Invokables;

namespace Kagami.Library.Objects
{
   public class RuntimeInvokable : IInvokable
   {
      protected Parameters parameters;

      public RuntimeInvokable(int parameterCount, string image)
      {
         parameters = new Parameters(parameterCount);
         Image = image;
         Index = -1;
         Address = -1;
      }

      public int Index { get; set; }

      public int Address { get; set; }

      public Parameters Parameters => parameters;

      public string Image { get; }

      public bool Constructing => false;

      public bool RequiresFunctionFrame => false;
   }
}