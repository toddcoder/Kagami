using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;
using Char = Kagami.Library.Objects.Char;

namespace Kagami.Library.Classes
{
   public class CharClass : BaseClass
   {
      public override string Name => "Char";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         rangeMessages();

         messages["*"] = (obj, msg) => function<Char, Int>(obj, msg, (c, i) => c.Repeat(i.Value));
         messages["+"] = (obj, msg) => function<Char, IObject>(obj, msg, (c, i) => c.Add(i));
         messages["-"] = (obj, msg) => function<Char, IObject>(obj, msg, (c, i) => c.Subtract(i));
         messages["upper"] = (obj, msg) => function<Char>(obj, c => c.Upper());
         messages["lower"] = (obj, msg) => function<Char>(obj, c => c.Lower());
         messages["isUpper".get()] = (obj, msg) => function<Char>(obj, c => c.IsUpper);
         messages["isLower".get()] = (obj, msg) => function<Char>(obj, c => c.IsLower);
         messages["isAlphaDigit".get()] = (obj, msg) => function<Char>(obj, c => c.IsAlphaDigit);
         messages["isAlpha".get()] = (obj, msg) => function<Char>(obj, c => c.IsAlpha);
         messages["isDigit".get()] = (obj, msg) => function<Char>(obj, c => c.IsDigit);
         messages["isSpace".get()] = (obj, msg) => function<Char>(obj, c => c.IsSpace);
         messages["isVowel".get()] = (obj, msg) => function<Char>(obj, c => c.IsVowel);
         messages["isConsonant".get()] = (obj, msg) => function<Char>(obj, c => c.IsConsonant);
         messages["ord".get()] = (obj, msg) => function<Char>(obj, c => c.Ord);
      }

      public override void RegisterClassMessages()
      {
         base.RegisterClassMessages();

         classMessages["cr".get()] = (cls, msg) => (Char)'\r';
         classMessages["lf".get()] = (cls, msg) => (Char)'\n';
         classMessages["tab".get()] = (cls, msg) => (Char)'\t';
         classMessages["fromOrd"] = (cls, msg) => new Char((char)((Int)msg.Arguments[0]).Value);
      }
   }
}