namespace Kagami.Library.Classes
{
	public class MonadClass : BaseClass
	{
		public override string Name => "Monad";

		public override bool AssignCompatible(BaseClass otherClass)
		{
			return base.AssignCompatible(otherClass) || otherClass.Name == "Some" || otherClass.Name == "None" ||
				otherClass.Name == "Success" || otherClass.Name == "Failure";
		}

		public override bool MatchCompatible(BaseClass otherClass) => AssignCompatible(otherClass);
	}
}