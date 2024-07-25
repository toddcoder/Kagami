namespace Kagami.Library.Invokables
{
   public class DataComparisandInvokable : FunctionInvokable
   {
      public DataComparisandInvokable(string functionName, Parameters parameters, string image) : base(functionName, parameters, image) { }

      public override bool Constructing => true;
   }
}