using System;
using System.Linq;
using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
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
      public Sys()
      {
         fields.New("id", new RuntimeLambda(args => args[0], 1, "x -> x"));
      }

      public override string ClassName => "Sys";

      public IObject ID => fields["id"];

      public override void LoadTypes(Module module)
      {
         module.RegisterClass(new SysClass());
         module.RegisterClass(new RegexMatchClass());
         module.RegisterClass(new RegexGroupClass());
         module.RegisterClass(new RandomClass());
      }

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
/*               Machine.Current.CurrentFrame.Fields.SetBindings(bindings, mutable, strict);
               return Boolean.True.Success();*/
               if (strict)
               {
                  foreach (var (key, value) in bindings)
                     if (Machine.Current.Find(key, true).If(out var field) && mutable == field.Mutable)
                        field.Value = value;
               }
               else
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

      public IObject Tuple(IObject value) => new Tuple(value);

      public RegexGroup RegexGroup(Arguments arguments)
      {
         var passed = new Hash<string, IObject>
         {
            ["text"] = arguments[0],
            ["index"] = arguments[1],
            ["length"] = arguments[2]
         };

         return new RegexGroup(passed);
      }

      public RegexMatch RegexMatch(Arguments arguments)
      {
         var passed = new Hash<string, IObject>
         {
            ["text"] = arguments[0],
            ["index"] = arguments[1],
            ["length"] = arguments[2],
            ["groups"] = arguments[3]
         };

         return new RegexMatch(passed);
      }

      public XRandom Random() => new XRandom();

      public XRandom Random(int seed) => new XRandom(seed);

      public Complex Complex(IObject real, IObject imaginary)
      {
         if (real is INumeric rNumeric)
         {
            var doubleR = rNumeric.AsDouble();
            if (imaginary is INumeric iNumeric)
            {
               var doubleI = iNumeric.AsDouble();
               return new Complex(doubleR, doubleI);
            }
            else
               throw notNumeric(imaginary);
         }
         else
            throw notNumeric(real);
      }

      public Selector Selector(string source) => source;

	   public Dictionary XFields()
	   {
		   return new Dictionary(Machine.Current.CurrentFrame.Fields.ToHash(t => String.StringObject(t.fieldName), t => t.field.Value));
	   }
   }
}