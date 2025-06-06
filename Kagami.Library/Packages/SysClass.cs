using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Packages;

public class SysClass : PackageClass
{
   public override string Name => "Sys";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      for (var i = 0; i < 10; i++)
      {
         registerPackageFunction("println".Selector(i), (obj, msg) => function<Sys>(obj, sys => sys.Println(msg.Arguments)));
      }

      registerPackageFunction("print(_)", (obj, msg) => function<Sys>(obj, sys => sys.Print(msg.Arguments)));
      registerPackageFunction("put(_)", (obj, msg) => function<Sys>(obj, sys => sys.Put(msg.Arguments)));
      registerPackageFunction("put(_,_)", (obj, msg) => function<Sys>(obj, sys => sys.Put(msg.Arguments)));
      registerPackageFunction("readln()", (obj, _) => function<Sys>(obj, sys => sys.Readln()));
      registerPackageFunction("peek(_)", (obj, msg) => function<Sys>(obj, sys => sys.Peek(msg.Arguments[0])));
      registerPackageFunction("peek(_,_)", (obj, msg) => function<Sys>(obj, sys => sys.Peek(msg.Arguments[0], msg.Arguments[1])));
      registerPackageFunction("ticks()", (obj, _) => function<Sys>(obj, sys => sys.Ticks()));
      registerPackageFunction("fst(_)", (obj, msg) => function<Sys, KTuple>(obj, msg, (sys, t) => sys.First(t)));
      registerPackageFunction("snd(_)", (obj, msg) => function<Sys, KTuple>(obj, msg, (sys, t) => sys.Second(t)));
      registerPackageFunction("id".get(), (obj, _) => function<Sys>(obj, sys => sys.ID));
      registerPackageFunction("Tuple(_)", (obj, msg) => function<Sys>(obj, sys => sys.Tuple(msg.Arguments[0])));
      registerPackageFunction("Tuple(_,_)", (obj, msg) => function<Sys>(obj, sys => sys.Tuple(msg.Arguments[0], msg.Arguments[1])));
      registerPackageFunction("Group(_,_,_)", (obj, msg) => function<Sys>(obj, sys => sys.RegexGroup(msg.Arguments)));
      registerPackageFunction("Match(_,_,_,_)", (obj, msg) => function<Sys>(obj, sys => sys.RegexMatch(msg.Arguments)));
      registerPackageFunction("Random()", (obj, _) => function<Sys>(obj, sys => sys.Random()));
      registerPackageFunction("Random(seed:<Int>)",
         (obj, msg) => function<Sys, Int>(obj, msg, (sys, i) => sys.Random(i.Value)));
      registerPackageFunction("Complex(_,_)", (obj, msg) => function<Sys, IObject, IObject>(obj, msg, (s, o1, o2) => s.Complex(o1, o2)));
      registerPackageFunction("sel(_)", (obj, msg) => function<Sys, KString>(obj, msg, (sys, s) => sys.Selector(s.Value)));
      registerPackageFunction("fields()", (obj, _) => function<Sys>(obj, sys => sys.XFields()));
      registerPackageFunction("Date(_<Float>)", (obj, msg) => function<Sys, Float>(obj, msg, (sys, f) => sys.Date(f.Value)));
      registerPackageFunction("Regex(_<String>)", (obj, msg) => function<Sys, KString>(obj, msg, (sys, s) => sys.Regex(s.Value)));
   }
}