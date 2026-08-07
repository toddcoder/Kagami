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
      messages["~(_)"] = (obj, msg) => function<KChar, KString>(obj, msg, (c1, c2) => (KString)(c1.AsString + c2.AsString));
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
      messages["isLetter".get()] = (obj, _) => function<KChar>(obj, c => c.IsLetter);
      messages["ord".get()] = (obj, _) => function<KChar>(obj, c => c.Ord);
      messages["byte()"] = (obj, _) => function<KChar>(obj, c => c.Byte());
      messages["succ".get()] = (obj, _) => function<KChar>(obj, c => c.Succ());
      messages["pred".get()] = (obj, _) => function<KChar>(obj, c => c.Pred());
      messages["unicodeCat".get()] = (obj, _) => function<KChar>(obj, c => c.UnicodeCat);
      messages["numberize()"] = (obj, _) => function<KChar>(obj, c => c.Numberize());
   }

   public override void RegisterClassMessages()
   {
      base.RegisterClassMessages();

      classMessages["cr".get()] = (_, _) => (KChar)'\r';
      classMessages["lf".get()] = (_, _) => (KChar)'\n';
      classMessages["tab".get()] = (_, _) => (KChar)'\t';
      classMessages["from(ord:_<Int>)"] = (_, msg) => classFunc<BaseClass, Int>(this, msg, (_, i) => fromOrd(i.Value));
   }

   public override IObject DefaultValue => new KChar((char)0);

   protected static IObject fromOrd(int value)
   {
      try
      {
         return Success.Object(KChar.CharObject((char)value));
      }
      catch (Exception exception)
      {
         return new Failure(exception.Message);
      }
   }

   public TypeConstraint EquivalentTypeConstraint() => TypeConstraint.FromList("TextFinding");
}