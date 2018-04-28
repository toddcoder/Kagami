namespace Kagami.Library.Invokables
{
	public class FunctionInvokable : IInvokable
	{
		protected string functionName;

		public FunctionInvokable(string functionName, Parameters parameters, string image)
		{
			this.functionName = functionName;
			Parameters = parameters;
			Image = image;
		}

		public string FunctionName => functionName;

		public int Index { get; set; } = -1;

		public int Address { get; set; } = -1;

		public Parameters Parameters { get; }

		public string Image { get; }

	   public virtual bool Constructing => false;

	   public override string ToString() => Image;
	}
}