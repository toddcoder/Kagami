using System;
using System.Collections.Generic;

namespace Kagami.Library.Objects
{
	public class MapIfAction : IStreamAction
	{
		protected Lambda lambda;
		protected Lambda predicate;

		public MapIfAction(Lambda lambda, Lambda predicate)
		{
			this.lambda = lambda;
			this.predicate = predicate;
		}

		public ILazyStatus Next(ILazyStatus status)
		{
			try
         {
            return status.IsAccepted ? Accepted.New(predicate.Invoke(status.Object).IsTrue ? lambda.Invoke(status.Object) : status.Object) : status;
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