using Core.Monads;

namespace Kagami.Library.Runtime;

public interface IContext
{
   void Print(string value);

   void PrintLine(string value);

   void Put(string value);

   void Put(string value, string separator);

   Result<string> ReadLine();

   bool Cancelled();

   int WriteCount { get; set; }
}