using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class LinesIterator : Iterator
{
   protected string input;

   public LinesIterator(KString kString) : base(kString)
   {
      input = kString.Value;
   }

   public override Maybe<IObject> Next()
   {
      var _currentPosition = input.FindByRegex("/r/n | /r | /n").Map(s => s.Index);
      if (_currentPosition is (true, var currentPosition))
      {
         var result = input.Keep(currentPosition);
         input = input.Drop(currentPosition).TrimStart('\r', '\n');

         return KString.StringObject(result).Some();
      }
      else if (input.IsNotEmpty())
      {
         var _result = KString.StringObject(input).Some();
         input = "";

         return _result;
      }
      else
      {
         return nil;
      }
   }
}