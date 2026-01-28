using Kagami.Library.Objects;

namespace Kagami.Library.Nodes.Statements;

public interface IAnnotatable
{
   Selector Selector { get; }

   Lambda Lambda { get; }

   void Fix();
}