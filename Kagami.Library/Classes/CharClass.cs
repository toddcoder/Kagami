using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;
using Char = Kagami.Library.Objects.Char;

namespace Kagami.Library.Classes
{
   public class CharClass : BaseClass, IEquivalentClass
   {
      public override string Name => "Char";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         rangeMessages();

         messages["*(_)"] = (obj, msg) => function<Char, Int>(obj, msg, (c, i) => c.Repeat(i.Value));
         messages["+(_)"] = (obj, msg) => function<Char, IObject>(obj, msg, (c, i) => c.Add(i));
         messages["-(_)"] = (obj, msg) => function<Char, IObject>(obj, msg, (c, i) => c.Subtract(i));
         messages["upper()"] = (obj, _) => function<Char>(obj, c => c.Upper());
         messages["lower()"] = (obj, _) => function<Char>(obj, c => c.Lower());
         messages["isUpper".get()] = (obj, _) => function<Char>(obj, c => c.IsUpper);
         messages["isLower".get()] = (obj, _) => function<Char>(obj, c => c.IsLower);
         messages["isAlphaDigit".get()] = (obj, _) => function<Char>(obj, c => c.IsAlphaDigit);
         messages["isAlpha".get()] = (obj, _) => function<Char>(obj, c => c.IsAlpha);
         messages["isDigit".get()] = (obj, _) => function<Char>(obj, c => c.IsDigit);
         messages["isSpace".get()] = (obj, _) => function<Char>(obj, c => c.IsSpace);
         messages["isVowel".get()] = (obj, _) => function<Char>(obj, c => c.IsVowel);
         messages["isConsonant".get()] = (obj, _) => function<Char>(obj, c => c.IsConsonant);
         messages["ord".get()] = (obj, _) => function<Char>(obj, c => c.Ord);
         messages["byte()"] = (obj, _) => function<Char>(obj, c => c.Byte());
      }

      public override void RegisterClassMessages()
      {
         base.RegisterClassMessages();

         classMessages["cr".get()] = (_, _) => (Char)'\r';
         classMessages["lf".get()] = (_, _) => (Char)'\n';
         classMessages["tab".get()] = (_, _) => (Char)'\t';
         classMessages["fromOrd"] = (_, msg) => new Char((char)((Int)msg.Arguments[0]).Value);
      }

      public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("TextFinding");
   }
}