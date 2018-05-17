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
      Hash<string, Signature> signatures;
      Hash<string, IInvokable> invokables;
      Hash<string, UserClass> implementors;

      public TraitClass(string traitName)
      {
         this.traitName = traitName;
         signatures = new Hash<string, Signature>();
         invokables = new Hash<string, IInvokable>();
         implementors = new Hash<string, UserClass>();
      }

      public override string Name => traitName;

      public IResult<Unit> RegisterSignature(Signature signature)
      {
         if (signatures.ContainsKey(signature.FullFunctionName))
            return $"Signature {signature.FullFunctionName} already exists in trait {traitName}".Failure<Unit>();
         else
         {
            signatures[signature.FullFunctionName] = signature;
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
         if (userClass.MatchImplemented(signatures.ValueArray()).If(out var signature, out var isNotMatched, out var exception))
            return $"Signature {signature.FullFunctionName} not implemented".Failure<Unit>();
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
         foreach (var signature in sourceTraitClass.signatures.ValueArray())
            if (RegisterSignature(signature).IfNot(out var exception))
               return failure<Unit>(exception);

         foreach (var (functionName, invokable) in sourceTraitClass.invokables)
            if (RegisterInvokable(functionName, invokable).If(out _, out var exception))
            {
               if (signatures.ContainsKey(functionName))
                  signatures.Remove(functionName);
            }
            else
               return failure<Unit>(exception);

         return Unit.Success();
      }
   }
}