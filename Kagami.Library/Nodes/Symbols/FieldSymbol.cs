using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class FieldSymbol : Symbol
{
   protected string fieldName;

   public FieldSymbol(string fieldName) => this.fieldName = fieldName;

   public override void Generate(OperationsBuilder builder) => builder.Field(this);

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => fieldName;

   public string FieldName => fieldName;
}