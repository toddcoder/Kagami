namespace Kagami.Library.Objects
{
   public interface ITextFinding
   {
      IObject Find(string input, int startIndex, bool reverse);

      KTuple FindAll(string input);

      KString Replace(string input, string replacement, bool reverse);

      KString Replace(string input, Lambda lambda, bool reverse);

      KString ReplaceAll(string input, string replacement);

      KString ReplaceAll(string input, Lambda lambda);

      KTuple Split(string input);

      KTuple Partition(string input, bool reverse);

      Int Count(string input);

      Int Count(string input, Lambda lambda);
   }
}