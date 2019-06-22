using System;
using System.Collections.Generic;

namespace Kagami.Library.Objects
{
   public class MapAction : IStreamAction
   {
      Lambda lambda;

      public MapAction(Lambda lambda) => this.lambda = lambda;

      public ILazyStatus Next(ILazyStatus status)
      {
         try
         {
            if (status.IsAccepted)
            {
	            return Accepted.New(lambda.Invoke(status.Object));
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
	         yield return lambda.Invoke(value);
         }
      }

      public override string ToString() => $"map {lambda.Image}";
   }
}