using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class GetReferenceSymbol : Symbol
{
   protected string fieldName;

   public GetReferenceSymbol(string fieldName)
   {
      this.fieldName = fieldName;
   }

   public override void Generate(OperationsBuilder builder) => builder.GetReference(fieldName);

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"ref {fieldName}";
}