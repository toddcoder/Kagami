﻿using Kagami.Library.Operations;
using Standard.Types.Maybe;

namespace Kagami.Library.Nodes.Symbols
{
	public class SkipTakeInitSymbol : SkipTakeSymbol
	{
		public SkipTakeInitSymbol(IMaybe<Expression> skip, IMaybe<Expression> take) : base(skip, take) { }

		public override void Generate(OperationsBuilder builder)
		{
			builder.NewSkipTake();

			base.Generate(builder);
		}
	}
}