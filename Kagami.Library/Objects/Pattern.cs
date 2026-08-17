using Core.Collections;
using Core.Enumerables;
using Core.Numbers;
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
   protected IObject[] argumentsToUse = [];

   public Pattern(string name, Lambda lambda, Parameters parameters)
   {
      this.name = name;
      this.lambda = lambda;
      this.parameters = parameters;

      foreach (var parameter in parameters)
      {
         fields.New(parameter.Name, FieldType.Parameter, true);
      }
   }

   public string ClassName => "Pattern";

   public string AsString => name;

   public string Image => $"pattern {name}({lambda.Invokable.Parameters.Select(p => p.Name).ToString(", ")})";

   public int Hash => Image.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Pattern pattern && name == pattern.name && lambda.IsEqualTo(pattern.lambda);

   protected string getPlaceholder(int index) => arguments[index].AsString;

   protected bool isPlaceholder(int index) => arguments[index] is Placeholder;

   protected IObject getValue(int index) => arguments[index];

   public bool Match(IObject comparisand, Hash<string, IObject> bindings)
   {
      lambda.CopyFields(fields);
      var result = lambda.Invoke([comparisand, ..argumentsToUse]);

      switch (result)
      {
         case KBoolean boolean when arguments.Length == 0:
            return boolean.Value;
         case KBoolean boolean when arguments.Length == 1:
         {
            if (boolean.IsTrue)
            {
               if (isPlaceholder(0))
               {
                  bindings[getPlaceholder(0)] = comparisand;
               }
               else if (!getValue(0).Match(comparisand, bindings))
               {
                  return false;
               }

               return true;
            }

            return false;
         }
         case Some { Value: KTuple kTuple }:
         {
            var length = kTuple.Length.Value.MinOf(arguments.Length);
            for (var i = 0; i < length; i++)
            {
               if (isPlaceholder(i))
               {
                  bindings[getPlaceholder(i)] = kTuple[i];
               }
               else if (!getValue(i).Match(kTuple[i], bindings))
               {
                  return false;
               }
            }

            return true;
         }
         case Some some:
         {
            if (isPlaceholder(0))
            {
               bindings[getPlaceholder(0)] = some.Value;
            }
            else if (!getValue(0).Match(some.Value, bindings))
            {
               return false;
            }

            return true;
         }
         case Success { Value: KTuple kTuple }:
         {
            var length = kTuple.Length.Value.MinOf(arguments.Length);
            for (var i = 0; i < length; i++)
            {
               if (isPlaceholder(i))
               {
                  bindings[getPlaceholder(i)] = kTuple[i];
               }
               else if (!getValue(i).Match(kTuple[i], bindings))
               {
                  return false;
               }
            }

            return true;
         }
         case Success success:
         {
            if (isPlaceholder(0))
            {
               bindings[getPlaceholder(0)] = success.Value;
            }
            else if (!getValue(0).Match(success.Value, bindings))
            {
               return false;
            }

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
      this.arguments = arguments.Value;
      Parameter[] parametersToUse = [.. parameters.Skip(1)];
      argumentsToUse = [.. arguments.Take(parametersToUse.Length)];
      this.arguments = [.. arguments.Value.Skip(argumentsToUse.Length)];

      foreach (var (parameter, argument) in parametersToUse.Zip(argumentsToUse))
      {
         fields.Assign(parameter.Name, argument);
      }
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
            fields.New(keyString.Value, FieldType.Assignment, value);
         }
      }

      return this;
   }
}