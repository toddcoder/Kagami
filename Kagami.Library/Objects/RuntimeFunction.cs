using System;
using Standard.Types.Collections;

namespace Kagami.Library.Objects
{
   public class RuntimeFunction : IObject, IMayInvoke
   {
      Func<IObject[], IObject> func;
      string image;

      public RuntimeFunction(Func<IObject[], IObject> func, string image)
      {
         this.func = func;
         this.image = image;
      }

      public string ClassName => "RuntimeFunction";

      public string AsString => image;

      public string Image => image;

      public int Hash => func.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is RuntimeFunction rf && image == rf.image;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => false;

      public bool IsTrue => false;

      public IObject Invoke(IObject[] arguments) => func(arguments);

      public IObject Join(Lambda lambda) => new RuntimeFunction(args => lambda.Invoke(Invoke(args)), $"{Image} >> {lambda.Image}");
   }
}