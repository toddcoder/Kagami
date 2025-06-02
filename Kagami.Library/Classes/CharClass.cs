using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class CharClass : BaseClass, IEquivalentClass
{
   public override string Name => "Char";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      rangeMessages();

      messages["*(_)"] = (obj, msg) => function<KChar, Int>(obj, msg, (c, i) => c.Repeat(i.Value));
      messages["+(_)"] = (obj, msg) => function<KChar, IObject>(obj, msg, (c, i) => c.Add(i));
      messages["-(_)"] = (obj, msg) => function<KChar, IObject>(obj, msg, (c, i) => c.Subtract(i));
      messages["upper()"] = (obj, _) => function<KChar>(obj, c => c.Upper());
      messages["lower()"] = (obj, _) => function<KChar>(obj, c => c.Lower());
      messages["isUpper".get()] = (obj, _) => function<KChar>(obj, c => c.IsUpper);
      messages["isLower".get()] = (obj, _) => function<KChar>(obj, c => c.IsLower);
      messages["isAlphaDigit".get()] = (obj, _) => function<KChar>(obj, c => c.IsAlphaDigit);
      messages["isAlpha".get()] = (obj, _) => function<KChar>(obj, c => c.IsAlpha);
      messages["isDigit".get()] = (obj, _) => function<KChar>(obj, c => c.IsDigit);
      messages["isSpace".get()] = (obj, _) => function<KChar>(obj, c => c.IsSpace);
      messages["isVowel".get()] = (obj, _) => function<KChar>(obj, c => c.IsVowel);
      messages["isConsonant".get()] = (obj, _) => function<KChar>(obj, c => c.IsConsonant);
      messages["ord".get()] = (obj, _) => function<KChar>(obj, c => c.Ord);
      messages["byte()"] = (obj, _) => function<KChar>(obj, c => c.Byte());
      messages["succ()"] = (obj, _) => function<KChar>(obj, c => c.Succ());
      messages["pred()"] = (obj, _) => function<KChar>(obj, c => c.Pred());
   }

   public override void RegisterClassMessages()
   {
      base.RegisterClassMessages();

      classMessages["cr".get()] = (_, _) => (KChar)'\r';
      classMessages["lf".get()] = (_, _) => (KChar)'\n';
      classMessages["tab".get()] = (_, _) => (KChar)'\t';
      classMessages["fromOrd"] = (_, msg) => new KChar((char)((Int)msg.Arguments[0]).Value);
   }

   public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("TextFinding");
}