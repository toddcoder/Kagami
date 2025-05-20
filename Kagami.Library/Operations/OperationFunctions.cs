using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public static class OperationFunctions
{
   public static Result<IIterator> getIterator(IObject value, bool lazy)
   {
      switch (value)
      {
         case ICollection collection:
            return collection.GetIterator(lazy).Success();
         case IIterator iterator:
            return iterator.Success();
         case Int i:
            return new KRange((Int)0, i, false).GetIterator(lazy).Success();
         case UserObject uo:
            var objectCollection = new ObjectCollection(uo);
            return objectCollection.GetIterator(lazy).Success();
         case Container internalList:
            var array = new KArray(internalList.List);
            return array.GetIterator(lazy).Success();
         default:
            return fail($"{value.Image} isn't an iterator nor can it return one");
      }
   }

   public static IObject sendMessage(IObject obj, string message, params IObject[] arguments)
   {
      return classOf(obj).SendMessage(obj, message, new Arguments(arguments));
   }

   public static IObject copyFields(IObject obj, FrameGroup frameGroup)
   {
      IObject value;
      if (obj is IPristineCopy pc)
      {
         value = pc.Copy();
      }
      else
      {
         value = obj;
      }

      if (value is ICopyFields cf)
      {
         cf.CopyFields(frameGroup.Fields);
      }

      return value;
   }

   public static IObject copyFields(IObject obj)
   {
      var frames = Machine.Current.PeekFrames(f => f.FrameType == FrameType.Function);
      if (frames.FunctionFrameIndex == -1)
      {
         frames.FunctionFrameIndex = frames.Count - 1;
      }

      return copyFields(obj, frames);
   }
}