using System;
using System.Collections.Generic;
using static Kagami.Library.Objects.CollectionFunctions;

namespace Kagami.Library.Objects
{
   public class MapAction : IStreamAction
   {
      protected Lambda lambda;

      public MapAction(Lambda lambda) => this.lambda = lambda;

      public ILazyStatus Next(ILazyStatus status)
      {
         try
         {
            if (status.IsAccepted)
            {
               var value = spread(status.Object);
	            return Accepted.New(lambda.Invoke(value));
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
            var wasSpread = spread(value);
	         yield return lambda.Invoke(wasSpread);
         }
      }

      public override string ToString() => $"map {lambda.Image}";
   }
}