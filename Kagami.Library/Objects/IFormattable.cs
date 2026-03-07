namespace Kagami.Library.Objects;

public interface IFormattable
{
   KString Format(string format);

   KString Format(string[] formats);

   KString Format(Lambda lambda);
}