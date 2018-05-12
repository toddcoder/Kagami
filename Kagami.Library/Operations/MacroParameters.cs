using System;
using System.Collections.Generic;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Collections;

namespace Kagami.Library.Operations
{
   public class MacroParameters
   {
      Hash<string, Expression> values;
/*      string tupleName;
      List<Expression> tuple;*/
      Hash<string, IInvokable> defaults;

      public MacroParameters()
      {
         values = new Hash<string, Expression>();
/*         tupleName = "";
         tuple = new List<Expression>();*/
         defaults = new Hash<string, IInvokable>();
      }

      public void Assign(Parameters parameters, Expression[] arguments)
      {
         var length = Math.Min(arguments.Length, parameters.Length);
         var lastValue = new Expression(new NilSymbol());
         //var lastName = "";
         var variadic = false;

         for (var i = 0; i < length && !variadic; i++)
         {
            var parameter = parameters[i];
            lastValue = arguments[i];
            values[parameter.Name] = lastValue;
            //lastName = parameter.Name;
            variadic = parameter.Variadic;
         }

         if (variadic)
         {
            var list = new List<Expression> { lastValue };
            for (var i = length; i < arguments.Length; i++)
               list.Add(arguments[i]);

/*            tupleName = lastName;
            tuple = list;*/
         }
         else if (length < parameters.Length)
            for (var i = length; i < parameters.Length; i++)
            {
               var parameter = parameters[i];
               var defaultValue = parameter.DefaultValue;
               if (defaultValue.If(out var invokable))
                  defaults[parameter.Name] = invokable;
            }
         else if (length < arguments.Length)
         {
            var list = new List<Expression> { lastValue };
            for (var i = length; i < arguments.Length; i++)
               list.Add(arguments[i]);

/*            tupleName = lastName;
            tuple = list;*/
         }
      }

      public bool Replace(Symbol symbol, OperationsBuilder builder)
      {
         switch (symbol)
         {
            case FieldSymbol fieldSymbol:
               return generate(fieldSymbol.FieldName, builder);
            default:
               return false;
         }
      }

      bool generate(string name, OperationsBuilder builder)
      {
         if (values.ContainsKey(name))
         {
            values[name].Generate(builder);
            return true;
         }
         else
            return false;
      }
   }
}