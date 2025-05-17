using System.Collections.Generic;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class Rotate : Operation
{
   protected int count;

   public Rotate(int count) => this.count = count;

   public override Optional<IObject> Execute(Machine machine)
   {
      List<IObject> list = [];
      for (var i = 0; i < count; i++)
      {
         var _value = machine.Pop();
         if (_value is (true, var value))
         {
            list.Add(value);
         }
         else
         {
            return _value.Exception;
         }
      }

      foreach (var value in list)
      {
         machine.Push(value);
      }

      return nil;
   }

   public override string ToString() => $"rotate({count})";
}