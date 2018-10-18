using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Classes
{
   public class TraitClass : BaseClass
   {
      string traitName;
      Set<Selector> signatures;
      Hash<string, IInvokable> invokables;
      Hash<string, UserClass> implementors;

      public TraitClass(string traitName)
      {
         this.traitName = traitName;
	      signatures = new Set<Selector>();
         invokables = new Hash<string, IInvokable>();
         implementors = new Hash<string, UserClass>();
      }

      public override string Name => traitName;

      public IResult<Unit> RegisterSignature(Selector signature)
      {
         if (signatures.Contains(signature))
            return $"Signature {signature.Image} already exists in trait {traitName}".Failure<Unit>();
         else
         {
	         signatures.Add(signature);
            return Unit.Success();
         }
      }

      public IResult<Unit> RegisterInvokable(string functionName, IInvokable invokable)
      {
         if (invokables.ContainsKey(functionName))
            return $"Function {functionName} already exists in trait {traitName}".Failure<Unit>();
         else
         {
            invokables[functionName] = invokable;
            return Unit.Success();
         }
      }

      public IResult<Unit> RegisterImplementor(UserClass userClass)
      {
         if (userClass.MatchImplemented(signatures).If(out var signature, out var isNotMatched, out var exception))
            return $"Signature {signature.Image} not implemented".Failure<Unit>();
         else if (isNotMatched)
         {
            implementors[userClass.Name] = userClass;
            return registerFunctions(userClass);
         }
         else
            return failure<Unit>(exception);
      }

      public IResult<Unit> registerFunctions(UserClass userClass)
      {
         foreach (var item in invokables)
         {
            var functionName = item.Key;
            if (userClass.ClassRespondsTo(functionName))
               functionName = $"{traitName}_{functionName}";
            if (userClass.RegisterMethod(functionName, new Lambda(item.Value), false)) { }
            else
               return $"function {functionName} already implemented".Failure<Unit>();
         }

         return Unit.Success();
      }

      public IResult<Unit> CopyFrom(TraitClass sourceTraitClass)
      {
         foreach (var signature in sourceTraitClass.signatures)
            if (RegisterSignature(signature).IfNot(out var exception))
               return failure<Unit>(exception);

         foreach (var (functionName, invokable) in sourceTraitClass.invokables)
            if (RegisterInvokable(functionName, invokable).If(out _, out var exception))
            {
               if (signatures.Contains(functionName))
                  signatures.Remove(functionName);
            }
            else
               return failure<Unit>(exception);

         return Unit.Success();
      }

	   public Hash<string, IInvokable> Invokables => invokables;
   }
}