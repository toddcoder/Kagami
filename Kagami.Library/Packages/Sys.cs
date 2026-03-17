using System.Collections;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Kagami.Library.AllExceptions;
using ICollection = Kagami.Library.Objects.ICollection;

namespace Kagami.Library.Packages;

public class Sys : Package
{
   public static Lambda IdLambda => new RuntimeLambda(args => args[0], 1, "x -> x");

   public Sys()
   {
      fields.New("identity", FieldType.Package, IdLambda);
      fields.New("environment", FieldType.Package, Environment);
      fields.New("eol", FieldType.Package, (KString)System.Environment.NewLine);
   }

   public override string ClassName => "Sys";

   public IObject Identity => fields["identity"];

   public IObject Out => fields["out"];

   public IObject Eol => fields["eol"];

   public override void LoadTypesOnce(Module module)
   {
      module.RegisterClass(new SysClass());
      module.RegisterClass(new RegexMatchClass());
      module.RegisterClass(new RegexGroupClass());
      module.RegisterClass(new RandomClass());
      module.RegisterClass(new OutClass());

      fields.New("out", FieldType.Package, new Out());
   }

   public KString Println(Arguments arguments)
   {
      var value = arguments.Select(a => a.AsString).ToString(" ");
      Machine.Current.Context.PrintLine(value);

      return value;
   }

   public KString Print(Arguments arguments)
   {
      var value = arguments.Select(a => a.AsString).ToString(" ");
      Machine.Current.Context.Print(value);

      return value;
   }

   public KString Put(Arguments arguments)
   {
      switch (arguments.Length)
      {
         case 1:
         {
            var value = arguments[0].AsString;
            Machine.Current.Context.Put(value);

            return value;
         }
         case 2:
         {
            var value = arguments[0].AsString;
            var separator = arguments[1].AsString;
            Machine.Current.Context.Put(value, separator);

            return value;
         }
         default:
         {
            var value = arguments.Select(a => a.AsString).ToString(" ");

            foreach (var argument in arguments)
            {
               Machine.Current.Context.Put(argument.AsString);
            }

            return value;
         }
      }
   }

   public IObject Column(IObject obj, int count)
   {
      var value = obj.AsString;
      var context = Machine.Current.Context;
      context.Put(value);
      if (context.WriteCount == count - 1)
      {
         context.PrintLine("");
      }

      if (count > 0)
      {
         context.WriteCount = (context.WriteCount + 1) % count;
      }

      return KString.StringObject(value);
   }

   public IObject Readln()
   {
      return Machine.Current.Context.ReadLine()
         .Map(s => Success.Object(KString.StringObject(s)))
         .Recover(e => Failure.Object(e.Message));
   }

   public IObject ReadInt()
   {
      return Machine.Current.Context.ReadLine()
         .Map(s => s.Result().Int32())
         .Map(i => Success.Object(Int.IntObject(i)))
         .Recover(e => Failure.Object(e.Message));
   }

   public IObject ReadFloat()
   {
      return Machine.Current.Context.ReadLine()
         .Map(s => s.Result().Double())
         .Map(f => Success.Object(Float.FloatObject(f)))
         .Recover(e => Failure.Object(e.Message));
   }

   public IObject Peek(IObject obj)
   {
      Machine.Current.Context.PrintLine(obj.Image);
      return obj;
   }

   public IObject Peek(IObject prefix, IObject obj)
   {
      Machine.Current.Context.PrintLine($"{prefix.AsString}: {obj.Image}");
      return obj;
   }

   public Result<IObject> Match(IObject x, IObject y)
   {
      var bindings = new Hash<string, IObject>();
      if (x.Match(y, bindings))
      {
         Machine.Current.CurrentFrame.Fields.SetBindings(bindings);
         return KBoolean.True.Success();
      }
      else
      {
         return KBoolean.False.Success();
      }
   }

   public Long Ticks() => new(DateTime.Now.Ticks);

   public IObject First(KTuple kTuple) => kTuple[0];

   public IObject Second(KTuple kTuple) => kTuple[1];

   public Result<IObject> GetReference(string fieldName)
   {
      var _field = Machine.Current.Find(fieldName, true);
      if (_field is (true, var field))
      {
         return new Reference(field);
      }
      else if (_field.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return fieldNotFound(fieldName);
      }
   }

   public IObject Tuple(IObject value) => new KTuple(value);

   public IObject Tuple(IObject value1, IObject value2) => new KTuple(value1, value2);

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

   public XRandom Random() => new();

   public XRandom Random(int seed) => new(seed);

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
         {
            throw notNumeric(imaginary);
         }
      }
      else
      {
         throw notNumeric(real);
      }
   }

   public Selector Selector(string source) => source;

   public Dictionary XFields()
   {
      return new(Machine.Current.CurrentFrame.Fields.ToHash(t => KString.StringObject(t.fieldName), t => t.field.Value));
   }

   public Date Date(double floating) => DateTime.FromOADate(floating);

   public Regex Regex(string pattern)
   {
      var _result = pattern.Matches("';' /(['IiMmGgTt']+) $");
      if (_result is (true, var result))
      {
         var ignoreCase = false;
         var multiline = false;
         var global = false;
         var textOnly = true;
         foreach (var option in result.FirstGroup)
         {
            switch (option)
            {
               case 'I':
               case 'i':
                  ignoreCase = true;
                  break;
               case 'M':
               case 'm':
                  multiline = true;
                  break;
               case 'G':
               case 'g':
                  global = true;
                  break;
               case 'T':
               case 't':
                  textOnly = true;
                  break;
            }
         }

         pattern = pattern.Drop(-result.Length);
         return new Regex(pattern, ignoreCase, multiline, global, textOnly);
      }
      else
      {
         return new Regex(pattern, false, false, false, false);
      }
   }

   public KString String(IObject obj) => new(obj.AsString);

   public Dictionary Environment
   {
      get
      {
         Hash<IObject, IObject> hash = [];
         foreach (DictionaryEntry entry in System.Environment.GetEnvironmentVariables())
         {
            hash[KString.StringObject(entry.Key.ToString() ?? "")] = KString.StringObject(entry.Value?.ToString() ?? "");
         }

         return new Dictionary(hash);
      }
   }

   public Junction All(ICollection collection) => collection.GetIterator(false).JunctionAll();

   public Junction Any(ICollection collection) => collection.GetIterator(false).JunctionAny();

   public Junction One(ICollection collection) => collection.GetIterator(false).JunctionOne();

   public Junction None(ICollection collection) => collection.GetIterator(false).JunctionNone();

   public IObject Eval(string source)
   {
      var compiler = new Compiler(source, CompilerConfiguration.Empty, Machine.Current.Context);
      var machine = compiler.Generate().ForceValue();

      return machine.Execute().ForceValue();
   }

   public IObject Eval(string source, Dictionary dictionary)
   {
      var compiler = new Compiler(source, CompilerConfiguration.Empty, Machine.Current.Context);
      var machine = compiler.Generate().ForceValue();
      if (dictionary.Length.Value > 0)
      {
         Hash<string, IObject> hash = [];
         foreach (var (key, value) in dictionary.InternalHash)
         {
            if (key is KString fieldName)
            {
               hash[fieldName.Value] = value;
            }
         }

         machine.Assignments = hash;
      }

      return machine.Execute().ForceValue();
   }

   public IObject UniqueId() => KString.StringObject(Guid.NewGuid().ToString());
}