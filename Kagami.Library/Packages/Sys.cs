using System;
using System.Linq;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Dates.Now;
using Standard.Types.Enumerables;
using String = Kagami.Library.Objects.String;

namespace Kagami.Library.Packages
{
   public class Sys : Package
   {
      Random random;

      public Sys() => random = new Random(NowServer.Now.Millisecond);

      public override string ClassName => "Sys";

      public override void LoadTypes(Module module)
      {
         module.RegisterClass(new SysClass());
      }

      public Float Rand() => random.NextDouble();

      public String Println(Arguments arguments)
      {
         var value = arguments.Select(a => a.AsString).Listify(" ");
         Machine.Current.Context.PrintLine(value);

         return value;
      }

      public String Print(Arguments arguments)
      {
         var value = arguments.Select(a => a.AsString).Listify(" ");
         Machine.Current.Context.Print(value);

         return value;
      }

      public String Put(Arguments arguments)
      {
         var value = arguments.Select(a => a.AsString).Listify(" ");

         foreach (var argument in arguments)
            Machine.Current.Context.Put(argument.AsString);

         return value;
      }

      public IObject Readln() => Machine.Current.Context.ReadLine().FlatMap(s => new Some(String.Object(s)), () => Nil.NilValue);

      public IObject Peek(IObject obj)
      {
         Machine.Current.Context.PrintLine(obj.Image);
         return obj;
      }
   }
}