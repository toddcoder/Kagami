namespace Kagami.Library.Nodes.Statements;

public interface IFieldsStatement
{
   IEnumerable<IFieldStatement> FieldStatements();
}