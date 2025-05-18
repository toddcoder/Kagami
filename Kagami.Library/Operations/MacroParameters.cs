using System;
using System.Collections.Generic;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Symbols;
using Core.Collections;

namespace Kagami.Library.Operations;

public class MacroParameters
{
   protected Hash<string, Expression> values = [];
   protected Hash<string, IInvokable> defaults = [];

   public void Assign(Parameters parameters, Expression[] arguments)
   {
      var length = Math.Min(arguments.Length, parameters.Length);
      var lastValue = new Expression(new NoneSymbol());
      var variadic = false;

      for (var i = 0; i < length && !variadic; i++)
      {
         var parameter = parameters[i];
         lastValue = arguments[i];
         values[parameter.Name] = lastValue;
         variadic = parameter.Variadic;
      }

      if (variadic)
      {
         var list = new List<Expression> { lastValue };
         for (var i = length; i < arguments.Length; i++)
         {
            list.Add(arguments[i]);
         }
      }
      else if (length < parameters.Length)
      {
         for (var i = length; i < parameters.Length; i++)
         {
            var parameter = parameters[i];
            var defaultValue = parameter.DefaultValue;
            if (defaultValue is (true, var invokable))
            {
               defaults[parameter.Name] = invokable;
            }
         }
      }
      else if (length < arguments.Length)
      {
         var list = new List<Expression> { lastValue };
         for (var i = length; i < arguments.Length; i++)
         {
            list.Add(arguments[i]);
         }
      }
   }

   public bool Replace(Symbol symbol, OperationsBuilder builder) => symbol switch
   {
      FieldSymbol fieldSymbol => generate(fieldSymbol.FieldName, builder),
      _ => false
   };

   protected bool generate(string name, OperationsBuilder builder)
   {
      if (values.ContainsKey(name))
      {
         values[name].Generate(builder);
         return true;
      }
      else
      {
         return false;
      }
   }
}