using Kagami.Library.Nodes.Statements;

namespace Kagami.Library.Classes;

public class AbstractUserClass(string className, string parentClassName, IEnumerable<AbstractFunction> abstractFunctions) : UserClass(className, parentClassName)
{
   protected AbstractFunction[] abstractFunctions = [.. abstractFunctions];
}