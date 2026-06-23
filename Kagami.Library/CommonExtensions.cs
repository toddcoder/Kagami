using Core.Computers;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
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

      public string set(TypeConstraint typeConstraint) => $"{name}=(_{typeConstraint.Image})";

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

   extension<T>(List<T> list) where T : notnull
   {
      public bool RemoveLast()
      {
         if (list.Count == 0)
         {
            return false;
         }
         else
         {
            list.RemoveAt(list.Count - 1);
            return true;
         }
      }
   }

   extension(IFieldStatement fieldStatement)
   {
      public IEnumerable<Selector> Selectors()
      {
         if (fieldStatement.Mutable)
         {
            if (fieldStatement.TypeConstraint is (true, var typeConstraint))
            {
               yield return fieldStatement.Name.set(typeConstraint);
            }
            else
            {
               yield return fieldStatement.Name.set();
            }
         }

         yield return fieldStatement.Name.get();
      }
   }
}