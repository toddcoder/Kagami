using System.Collections.Generic;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using Standard.Types.Exceptions;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Invokables
{
   public class YieldingInvokable : IInvokable, ICollection, IObject
   {
      string functionName;
      List<IObject> cached;

      public YieldingInvokable(string functionName, Parameters parameters, string image)
      {
         this.functionName = functionName;
         Parameters = parameters;
         Image = image;
         cached = new List<IObject>();
      }

      public string FunctionName => functionName;

      public int Index { get; set; } = -1;

      public int Address { get; set; } = -1;

      public Parameters Parameters { get; }

      public string ClassName => "YieldingInvokable";

      public string AsString => functionName;

      public string Image { get; }

      public bool Constructing => false;

      public int Hash => functionName.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is YieldingInvokable yfi && functionName == yfi.functionName;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => false;

      public bool IsTrue => false;

      public Arguments Arguments { get; set; } = Arguments.Empty;

      public FrameGroup Frames { get; set; } = new FrameGroup();

      public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

      public IMaybe<IObject> Next(int index)
      {
         var result = Machine.Current.Invoke(this).Value;
         switch (result)
         {
            case Nil _:
               return none<IObject>();
            case YieldReturn yr:
               Address = yr.Address + 1;
               Frames = yr.Frames;
               return yr.ReturnValue.Some();
            default:
               throw incompatibleClasses(result, "YieldReturn");
         }
      }

      public IMaybe<IObject> Peek(int index) => throw "Peek not supported".Throws();

      public Int Length => cached.Count;

      public IEnumerable<IObject> List
      {
         get
         {
            var iterator = GetIterator(false);
            cached.Clear();

            while (true)
            {
               var next = iterator.Next();
               if (next.If(out var value))
               {
                  cached.Add(value);
                  yield return value;
               }
               else
                  yield break;
            }
         }
      }

      public bool ExpandForArray => true;

      public Boolean In(IObject item) => cached.Contains(item);

      public Boolean NotIn(IObject item) => !cached.Contains(item);

      public IObject Times(int count) => this;
   }
}