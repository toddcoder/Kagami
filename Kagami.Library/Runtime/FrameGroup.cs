﻿using System;
using System.Collections;
using System.Collections.Generic;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Runtime
{
   public class FrameGroup : IEnumerable<Frame>
   {
      Frame[] frames;
      int functionFrameIndex;

      public FrameGroup(Frame[] frames)
      {
         this.frames = frames;
         functionFrameIndex = Array.FindIndex(frames, f => f.FrameType == FrameType.Function);
      }

      public FrameGroup()
      {
         frames = new Frame[0];
         functionFrameIndex = -1;
      }

      public void Push(Machine machine)
      {
         foreach (var frame in frames)
            machine.PushFrame(frame);
      }

      public IMaybe<Frame> FunctionFrame => when(functionFrameIndex > -1, () => frames[functionFrameIndex]);

      public IMaybe<Frame> ExitFrame => frames.FirstOrNone(f => f.FrameType == FrameType.Exit);

      public IMaybe<Frame> SkipFrame => frames.FirstOrNone(f => f.FrameType == FrameType.Skip);

      public IMaybe<Frame> TopFrame => when(frames.Length > 0, () => frames[0]);

      public IEnumerator<Frame> GetEnumerator()
      {
         foreach (var frame in frames)
            yield return frame;
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public Fields Fields
      {
         get
         {
            var fields = new Fields();

            for (var i = functionFrameIndex; i >= 0; i--)
               fields.CopyFrom(frames[i].Fields);

            return fields;
         }
      }
   }
}