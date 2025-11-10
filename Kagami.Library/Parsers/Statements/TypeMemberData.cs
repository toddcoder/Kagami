using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;

namespace Kagami.Library.Parsers.Statements;

public record TypeMemberData(string Name, Parameters Parameters, Maybe<IObject> Ordinal, Maybe<Block> Block);