﻿using Kagami.Library;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Awk
{
   public class AwkifierClass : BaseClass
   {
      public override string Name => "Awkifier";

      public override void RegisterMessages()
      {
         base.RegisterMessages();
         collectionMessages();

         registerMessage("recordPattern".get(), (obj, _) => function<Awkifier>(obj, a => a.RecordPattern));
         registerMessage("recordPattern".set(), (obj, msg) => function<Awkifier, Regex>(obj, msg, (a, r) => a.RecordPattern = r));
         registerMessage("fieldPattern".get(), (obj, _) => function<Awkifier>(obj, a => a.FieldPattern));
         registerMessage("fieldPattern".set(), (obj, msg) => function<Awkifier, Regex>(obj, msg, (a, r) => a.FieldPattern = r));
         registerMessage("recordSeparator".get(), (obj, _) => function<Awkifier>(obj, a => a.RecordSeparator));
         registerMessage("recordSeparator".set(), (obj, msg) => function<Awkifier, KString>(obj, msg, (a, r) => a.RecordSeparator = r));
         registerMessage("fieldSeparator".get(), (obj, _) => function<Awkifier>(obj, a => a.FieldSeparator));
         registerMessage("fieldSeparator".set(), (obj, msg) => function<Awkifier, KString>(obj, msg, (a, r) => a.FieldSeparator = r));
         registerMessage("[]", (obj, msg) => function<Awkifier, Int>(obj, msg, (a, i) => a[i.Value]));
      }

      public TypeConstraint TypeConstraint() => Library.Objects.TypeConstraint.FromList("Collection", "TextFinding", "String");
   }
}