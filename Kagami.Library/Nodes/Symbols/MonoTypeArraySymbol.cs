using Core.Matching;
using Core.Numbers;
using Core.Objects;
using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class MonoTypeArraySymbol(string source) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      var array = source.Trim().Unjoin("/s+; f");
      if (array.All(i => i.IsIntegral()))
      {
         var intArray = new KArray(array.Select(i => Int.IntObject(i.Value().Int32())));
         builder.PushObject(intArray);
      }
      else if (array.All(f => f.IsFloat()))
      {
         var floatArray = new KArray(array.Select(f => Float.FloatObject(f.Value().Double())));
         builder.PushObject(floatArray);
      }
      else
      {
         var stringArray = new KArray(array.Select(KString.StringObject));
         builder.PushObject(stringArray);
      }
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"<{source}>";
}