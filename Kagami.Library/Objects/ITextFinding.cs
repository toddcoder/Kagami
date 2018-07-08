namespace Kagami.Library.Objects
{
   public interface ITextFinding
   {
      IObject Find(string input, int startIndex, bool reverse);

      Tuple FindAll(string input);

      String Replace(string input, string replacement, bool reverse);

      String ReplaceAll(string input, string replacement);

      Tuple Split(string input);

      Tuple Partition(string input, bool reverse);
   }
}