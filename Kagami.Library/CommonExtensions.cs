using Kagami.Library.Objects;
using Core.Enumerables;
using Core.Monads;
using Core.Matching;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library;

public static class CommonExtensions
{
   extension(string name)
   {
      public string get() => $"__${name}()";

      public string unget() => name.Substitute("^ '__$' /(.*) $", "$1");

      public string set() => $"{name}=(_)";

      public Selector Selector(params string[] selectorItemSources)
      {
         if (selectorItemSources.Length == 0)
         {
            return new Selector(name);
         }
         else
         {
            var selectorItems = selectorItemSources.Select(parseSelectorItem).ToArray();
            var image = $"{name}({selectorItemSources.ToString(",")})";

            return new Selector(name, selectorItems, image);
         }
      }

      public Selector Selector(int count)
      {
         return name.Selector(Enumerable.Range(0, count).Select(_ => "_").ToArray());
      }
   }

   extension<T>(Maybe<T> maybe) where T : IObject
   {
      public IObject AsOptional()
      {
         return maybe.Map(o => Some.Object(o)) | (() => KNil.NilValue);
      }
   }

   extension(IOptional optional)
   {
      public Maybe<T> AsMaybe<T>() where T : IObject
      {
         return optional is Some some ? (Maybe<T>)some.Value : nil;
      }
   }

   extension(JunctionType junctionType)
   {
      public string OperatorString => junctionType switch
      {
         JunctionType.All => "&&",
         JunctionType.Any => "||",
         JunctionType.One => "^^",
         JunctionType.None => "!!",
         _ => ""
      };
   }
}