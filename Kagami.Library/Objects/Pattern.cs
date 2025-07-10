using Core.Collections;
using Core.DataStructures;
using Core.Enumerables;
using Kagami.Library.Invokables;
using Kagami.Library.Runtime;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class Pattern : IObject
{
   protected string name;
   protected Lambda lambda;
   protected Fields fields = new();
   protected Parameters parameters;
   protected IObject[] arguments = [];
   protected string placeholder;
   protected bool hasPlaceholder;

   public Pattern(string name, Lambda lambda, Parameters parameters)
   {
      this.name = name;
      this.lambda = lambda;
      this.parameters = parameters;

      placeholder = parameters.FirstOrNone().Map(p => p.Name) | "$0";
      hasPlaceholder = placeholder != "$0";

      foreach (var parameter in parameters)
      {
         fields.New(parameter.Name, true);
      }
   }

   public string ClassName => "Pattern";

   public string AsString => name;

   public string Image => $"pattern {name}({lambda.Invokable.Parameters.Select(p => p.Name).ToString(", ")})";

   public int Hash => Image.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Pattern pattern && name == pattern.name && lambda.IsEqualTo(pattern.lambda);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings)
   {
      lambda.CopyFields(fields);
      var result = lambda.Invoke(comparisand);

      switch (result)
      {
         case KBoolean boolean when hasPlaceholder:
         {
            if (boolean.IsTrue)
            {
               bindings[$"-{parameters[0].Name}"] = comparisand;
            }

            return boolean.IsTrue;
         }
         case KBoolean boolean:
            return boolean.Value;
         case Some some when hasPlaceholder:
         {
            bindings[$"-{placeholder}"] = some.Value;
            return true;
         }
         default:
            if (result is Some { Value: KTuple tuple } && tuple.Length.Value == arguments.Length)
            {
               return tuple.Value.Zip(arguments, (l, r) => match(l, r, bindings)).All(b => b);
            }

            break;
      }

      return false;
   }

   public void RegisterArguments(Arguments arguments)
   {
      var fieldValues = arguments.Take(fields.Length).ToArray();
      var parameterNames = parameters.Select(p => p.Name).ToArray();
      var index = 0;
      foreach (var parameterName in parameterNames)
      {
         fields.Assign(parameterName, fieldValues[index++]);
      }

      this.arguments = arguments.Skip(fields.Length).ToArray();
   }

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();

   public Pattern Copy() => new(name, lambda, parameters);

   public Pattern With(Dictionary dictionary)
   {
      foreach (var (key, value) in dictionary.InternalHash)
      {
         if (key is KString keyString)
         {
            fields.New(keyString.Value, value);
         }
      }

      return this;
   }

   public Pattern Invoke(Arguments arguments)
   {
      if (arguments.Length != parameters.Length)
      {
         throw new InvalidOperationException($"Pattern '{name}' expects {parameters.Length} arguments, but got {arguments.Length}.");
      }

      if (arguments.Length > 0)
      {
         IObject[] firstArgument = [.. arguments.Take(1)];
         fields.New(placeholder, firstArgument[0]);

         MaybeStack<IObject> remainingArguments = [.. arguments.Skip(1)];
         foreach (var parameter in parameters.Skip(1))
         {
            if (remainingArguments.Pop() is (true, var value))
            {
               fields.New(parameter.Name, value);
            }
         }
      }

      return this;
   }
}