using Kagami.Library.Runtime;
using Core.Collections;

namespace Kagami.Library.Objects;

public readonly struct YieldReturn : IObject
{
   private readonly IObject returnValue;
   private readonly int address;
   private readonly FrameGroup frames;

   public YieldReturn(IObject returnValue, int address, FrameGroup frames) : this()
   {
      this.returnValue = returnValue;
      this.address = address;
      this.frames = frames;
   }

   public IObject ReturnValue => returnValue;

   public int Address => address;

   public FrameGroup Frames => frames;

   public string ClassName => "YieldReturn";

   public string AsString => $"{returnValue.AsString} at {address}";

   public string Image => $"{returnValue.Image} at {address}";

   public int Hash => returnValue.Hash;

   public bool IsEqualTo(IObject obj) => returnValue.IsEqualTo(obj);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => returnValue.Match(comparisand, bindings);

   public bool IsTrue => true;
}