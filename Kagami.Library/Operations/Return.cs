using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class Return : Operation
   {
      public static IMatched<IObject> ReturnAction(Machine machine, bool returnTopOfStack)
      {
         var rtn = Machine.Current.CurrentFrame.Pop().FlatMap(o => o.Matched(), e => notMatched<IObject>());

         var frames = machine.PopFrames();
         if (frames.FunctionFrame.If(out var frame))
         {
            var returnAddress = frame.Address;
            if (returnTopOfStack)
               if (rtn.If(out var v, out var isNotMatched, out var exception))
               {
                  IObject v2;
                  if (v is IPristineCopy pc)
                     v2 = pc.Copy();
                  else
                     v2 = v;
                  if (v2 is ICopyFields cf)
                     cf.CopyFields(frames.Fields);
                  rtn = v2.Matched();
               }
               else if (isNotMatched)
                  return failedMatch<IObject>(emptyStack());
               else
                  return failedMatch<IObject>(exception);
            else
               rtn = notMatched<IObject>();

            if (machine.GoTo(returnAddress))
               return rtn;
            else
               return failedMatch<IObject>(badAddress(returnAddress));
         }
         else
            return failedMatch<IObject>(invalidStack());
      }

      bool returnTopOfStack;

      public Return(bool returnTopOfStack) => this.returnTopOfStack = returnTopOfStack;

      public override IMatched<IObject> Execute(Machine machine) => ReturnAction(machine, returnTopOfStack);

      public override bool Increment => false;

      public override string ToString() => $"return({returnTopOfStack.ToString().ToLower()})";

      public bool ReturnTopOfStack => returnTopOfStack;
   }
}