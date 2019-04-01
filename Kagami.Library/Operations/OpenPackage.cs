using Kagami.Library.Objects;
using Kagami.Library.Packages;
using Kagami.Library.Runtime;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
	public class OpenPackage : Operation
	{
		string packageName;

		public OpenPackage(string packageName) => this.packageName = packageName;

		public override IMatched<IObject> Execute(Machine machine)
		{
			var fieldName = packageName.ToLower1();
			if (machine.Find(fieldName, true).If(out var field, out var mbException))
				switch (field.Value)
				{
					case Package package when Module.Global.Class(package.ClassName).If(out var baseClass):
						if (baseClass is PackageClass packageClass)
						{
							packageClass.RegisterMessages();
							packageClass.CopyToGlobalFrame(package);

							return notMatched<IObject>();
						}
						else
							return failedMatch<IObject>(unableToConvert(baseClass.Name, "Package class"));
					case Package package:
						return failedMatch<IObject>(classNotFound(package.ClassName));
					default:
						return failedMatch<IObject>(unableToConvert(field.Value.Image, "Package"));
				}
			else if (mbException.If(out var exception))
				return failedMatch<IObject>(exception);
			else
				return failedMatch<IObject>(fieldNotFound(fieldName));
		}

		public override string ToString() => $"open.package({packageName})";
	}
}