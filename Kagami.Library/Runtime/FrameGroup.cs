using System.Collections;
using Core.Enumerables;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Runtime;

public class FrameGroup : IEnumerable<Frame>
{
   protected Frame[] frames;
   protected int functionFrameIndex;

   public FrameGroup(Frame[] frames)
   {
      this.frames = frames;
      functionFrameIndex = Array.FindIndex(frames, f => f.FrameType == FrameType.Function);
   }

   public FrameGroup(Frame[] frames, int functionFrameIndex)
   {
      this.frames = frames;
      this.functionFrameIndex = functionFrameIndex;
   }

   public FrameGroup()
   {
      frames = [];
      functionFrameIndex = -1;
   }

   public void Push(Machine machine)
   {
      foreach (var frame in frames)
      {
         machine.PushFrame(frame);
      }
   }

   public Maybe<Frame> FunctionFrame => maybe<Frame>() & functionFrameIndex > -1 & (() => frames[functionFrameIndex]);

   public int FunctionFrameIndex
   {
      get => functionFrameIndex;
      set => functionFrameIndex = value;
   }

   public int Count => frames.Length;

   public Maybe<Frame> ExitFrame => frames.FirstOrNone(f => f.FrameType == FrameType.Exit);

   public Maybe<Frame> SkipFrame => frames.FirstOrNone(f => f.FrameType == FrameType.Skip);

   public Maybe<Frame> TopFrame => maybe<Frame>() & frames.Length > 0 & (() => frames[0]);

   public Maybe<Frame> TryFrame => frames.FirstOrNone(f => f.FrameType == FrameType.Try);

   public IEnumerator<Frame> GetEnumerator()
   {
      foreach (var frame in frames)
      {
         yield return frame;
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public Fields Fields
   {
      get
      {
         var fields = new Fields();

         for (var i = functionFrameIndex; i >= 0; i--)
         {
            fields.CopyFrom(frames[i].Fields);
         }

         return fields;
      }
   }

   public string AsString => frames.Select(f => f.AsString).ToString(", ");
}