﻿using Kagami.Library.Objects;
using Standard.Types.Maybe;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class DictionaryClass : BaseClass
   {
      public override string Name => "Dictionary";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         collectionMessages();
         mutableCollectionMessages();

         messages["[]"] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, k) => d[k]);
         messages["[]="] = (obj, msg) => function<Dictionary>(obj, d => d[msg.Arguments[0]] = msg.Arguments[1]);
         messages["[]?"] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, k) => d.Get(k));
         messages["default".get()] = (obj, msg) => function<Dictionary>(obj, d =>
         {
            if (d.DefaultValue.If(out var dv))
               return dv;
            if (d.DefaultLambda.If(out var dl))
               return dl;

            return Unassigned.Value;
         });
         messages["default".set()] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, v) =>
         {
            if (v is Lambda lambda)
               d.DefaultLambda = lambda.Some();
            d.DefaultValue = v.Some();

            return Void.Value;
         });
         messages["caching".get()] = (obj, msg) => function<Dictionary>(obj, d => d.Caching);
         messages["caching".set()] = (obj, msg) => function<Dictionary, Boolean>(obj, msg, (d, b) => d.Caching = b);
         messages[">>"] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, k) => d.Delete(k));
         messages["keys".get()] = (obj, msg) => function<Dictionary>(obj, d => d.Keys);
         messages["values".get()] = (obj, msg) => function<Dictionary>(obj, d => d.Values);
         messages["in"] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, k) => d.In(k));
         messages["notIn"] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, k) => d.NotIn(k));
         messages["swap"] = (obj, msg) => function<Dictionary, IObject, IObject>(obj, msg, (d, k1, k2) => d.Swap(k1, k2));
         messages["clear"] = (obj, msg) => function<Dictionary>(obj, d => d.Clear());
         messages["update".Function("key", "value")] =
            (obj, msg) => function<Dictionary, IObject, IObject>(obj, msg, (d, k, v) => d.Update(k, v));
      }

      public override void RegisterClassMessages()
      {
         base.RegisterClassMessages();

         classMessages["new".Function("default", "caching")] = (cls, msg) => classFunc<DictionaryClass, IObject, Boolean>(cls, msg,
            (dc, d, c) => Dictionary.New(d, c));
      }
   }
}