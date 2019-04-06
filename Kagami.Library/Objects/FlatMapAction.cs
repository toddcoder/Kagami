using System;
using System.Collections.Generic;

namespace Kagami.Library.Objects
{
	public class FlatMapAction : IStreamAction
	{
		Lambda lambda;

		public FlatMapAction(Lambda lambda) => this.lambda = lambda;

		public ILazyStatus Next(ILazyStatus status)
		{
			try
			{
				if (status.IsAccepted)
					return Accepted.New(lambda.Invoke(status.Object));
				else
					return status;
			}
			catch (Exception exception)
			{
				return new Failed(exception);
			}
      }

		public IEnumerable<IObject> Execute(IIterator iterator)
		{
			var flattened = (ICollection)iterator.Flatten();
			var newIterator = flattened.GetIterator(false);
			foreach (var item in newIterator.List())
				yield return item;
		}
	}
}