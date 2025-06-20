﻿using Kagami.Library.Objects;
using Core.Enumerables;
using Core.Monads;
using Core.Matching;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library;

public static class CommonExtensions
{
   public static string get(this string name) => $"__${name}()";

   public static string unget(this string name) => name.Substitute("^ '__$' /(.*) $", "$1");

   public static string set(this string name) => $"{name}=(_)";

   public static Selector Selector(this string baseName, params string[] selectorItemSources)
   {
      if (selectorItemSources.Length == 0)
      {
         return new Selector(baseName);
      }
      else
      {
         var selectorItems = selectorItemSources.Select(parseSelectorItem).ToArray();
         var image = $"{baseName}({selectorItemSources.ToString(",")})";

         return new Selector(baseName, selectorItems, image);
      }
   }

   public static Selector Selector(this string baseName, int count)
   {
      return baseName.Selector(Enumerable.Range(0, count).Select(i => "_").ToArray());
   }

   public static IObject AsOptional<T>(this Maybe<T> maybe) where T : IObject
   {
      return maybe.Map(o => Some.Object(o)) | (() => None.NoneValue);
   }

   public static Maybe<T> AsMaybe<T>(this IOptional optional) where T : IObject
   {
      return optional is Some some ? (Maybe<T>)some.Value : nil;
   }
}