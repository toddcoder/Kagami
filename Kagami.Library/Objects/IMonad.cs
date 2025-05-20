namespace Kagami.Library.Objects
{
	public interface IMonad
	{
		IObject Bind(Lambda map);

		IObject Unit(IObject obj);

		KBoolean CanBind { get; }
	}
}