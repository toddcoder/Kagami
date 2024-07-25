namespace Kagami.Library.Objects
{
	public interface IResult
	{
		IObject Value { get; }

		Error Error { get; }

		bool IsSuccess { get; }

		bool IsFailure { get; }

		IObject Map(Lambda lambda);

		IObject FlatMap(Lambda ifSuccess, Lambda ifFailure);
	}
}