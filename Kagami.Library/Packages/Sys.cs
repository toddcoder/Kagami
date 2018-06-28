using System;
using System.Linq;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using Standard.Types.Dates.Now;
using Standard.Types.Enumerables;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;
using Boolean = Kagami.Library.Objects.Boolean;
using String = Kagami.Library.Objects.String;
using Tuple = Kagami.Library.Objects.Tuple;

namespace Kagami.Library.Packages
{
   public class Sys : Package
   {
      Random random;

      public Sys()
      {
         random = new Random(NowServer.Now.Millisecond);
         fields.New("id", new RuntimeLambda(args => args[0], 1, "x -> x"));
      }

      public override string ClassName => "Sys";

      public IObject ID => fields["id"];

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

      public IObject Readln() => Machine.Current.Context.ReadLine().FlatMap(s => new Some(String.StringObject(s)), () => Nil.NilValue);

      public IObject Peek(IObject obj)
      {
         Machine.Current.Context.PrintLine(obj.Image);
         return obj;
      }

      public IResult<IObject> Match(bool mutable, bool strict, IObject x, IObject y)
      {
         if (y is Pattern pattern)
            return MatchToPattern(pattern, x, mutable, strict);
         else
         {
            var bindings = new Hash<string, IObject>();
            if (x.Match(y, bindings))
            {
               Machine.Current.CurrentFrame.Fields.SetBindings(bindings, mutable, strict);
               return Boolean.True.Success();
            }
            else
               return Boolean.False.Success();
         }
      }

      public IResult<IObject> MatchToPattern(Pattern pattern, IObject source, bool mutable, bool strict)
      {
         try
         {
            var cases = pattern.Cases;
            foreach (var (comparisand, lambda) in cases)
            {
               var bindings = new Hash<string, IObject>();
               var frame = new Frame();
               Machine.Current.PushFrame(frame);
               if (source.Match(comparisand, bindings))
               {
                  frame.Fields.SetBindings(bindings, mutable, strict);
                  var result = lambda.Invoke();
                  frame.Pop();
                  return result.Success();
               }

               frame.Pop();
            }

            return "No match".Failure<IObject>();
         }
         catch (Exception exception)
         {
            return failure<IObject>(exception);
         }
      }

      public Long Ticks() => new Long(DateTime.Now.Ticks);

      public IResult<IObject> NewParameterlessObject(string className, string fieldName)
      {
         try
         {
            var userObject = new UserObject(className, new Fields(), Parameters.Empty);
            if (Machine.Current.CurrentFrame.Fields.New(fieldName, userObject).IfNot(out var exception))
               return failure<IObject>(exception);
            else
               return userObject.Success<IObject>();
         }
         catch (Exception exception)
         {
            return failure<IObject>(exception);
         }
      }

      public IObject First(Tuple tuple) => tuple[0];

      public IObject Second(Tuple tuple) => tuple[1];

      public IResult<IObject> GetReference(string fieldName)
      {
         return Machine.Current.Find(fieldName, true).FlatMap(f => new Reference(f).Success<IObject>(),
            () => failure<IObject>(fieldNotFound(fieldName)), failure<IObject>);
      }
   }
}