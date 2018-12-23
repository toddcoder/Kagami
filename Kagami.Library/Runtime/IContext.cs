using Standard.Types.Monads;

namespace Kagami.Library.Runtime
{
   public interface IContext
   {
      void Print(string value);

      void PrintLine(string value);

      void Put(string value);

      IMaybe<string> ReadLine();

      bool Cancelled();

      void Peek(string message, int index);
   }
}