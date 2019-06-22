using System;
using System.Collections.Generic;

namespace Kagami.Library.Objects
{
	public class MapIfAction : IStreamAction
	{
		Lambda lambda;
		Lambda predicate;

		public MapIfAction(Lambda lambda, Lambda predicate)
		{
			this.lambda = lambda;
			this.predicate = predicate;
		}

		public ILazyStatus Next(ILazyStatus status)
		{
			try
			{
				if (status.IsAccepted)
				{
					if (predicate.Invoke(status.Object).IsTrue)
					{
						return Accepted.New(lambda.Invoke(status.Object));
					}
					else
					{
						return Accepted.New(status.Object);
					}
				}
				else
				{
					return status;
				}
			}
			catch (Exception exception)
			{
				return new Failed(exception);
			}
		}

		public IEnumerable<IObject> Execute(IIterator iterator)
		{
			foreach (var value in iterator.List())
			{
				if (predicate.Invoke(value).IsTrue)
				{
					yield return lambda.Invoke(value);
				}
				else
				{
					yield return value;
				}
			}
		}
	}
}