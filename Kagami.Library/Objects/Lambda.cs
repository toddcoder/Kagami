using System;
using Kagami.Library.Invokables;
using Kagami.Library.Runtime;
using Core.Collections;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public class Lambda : IObject, IEquatable<Lambda>, IInvokableObject, ICopyFields, IPristineCopy, IProvidesFields
   {
      protected IInvokable invokable;
      protected Fields fields;
      protected bool providesFields;

      public Lambda(IInvokable invokable)
      {
         this.invokable = invokable;
         fields = new Fields();
         providesFields = false;
      }

      public string ClassName => "Lambda";

      public virtual string AsString => invokable.ToString();

      public virtual string Image => invokable.Image;

      public virtual int Hash => invokable.GetHashCode();

      public virtual bool IsEqualTo(IObject obj) => obj is Lambda l && invokable.Index == l.invokable.Index;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => true;

      public bool Equals(Lambda other) => IsEqualTo(other);

      public override bool Equals(object obj) => Equals((Lambda)obj);

      public override int GetHashCode() => Hash;

      public virtual IObject Copy() => new Lambda(invokable);

      public void CopyFields(Fields fields)
      {
         this.fields.CopyFrom(fields);
         providesFields = true;
      }

      public IInvokable Invokable => invokable;

      public virtual IObject Invoke(params IObject[] arguments)
      {
	      if (Machine.Current.Invoke(invokable, new Arguments(arguments), fields, 0).If(out var value, out var anyException))
	      {
		      return value;
	      }
	      else if (anyException.If(out var exception))
	      {
		      throw exception;
	      }
	      else
	      {
		      throw fieldNotFound(invokable.Image);
	      }
      }

      public bool ProvidesFields => providesFields;

      public Fields Fields => fields;

      public Lambda Clone() => new Lambda(invokable);

      public IObject Join(Lambda otherLambda)
      {
         //return new RuntimeLambda(args => otherLambda.Invoke(Invoke(args)), 1, $"{Image} >> {otherLambda.Image}");
         return new CompositeLambda(invokable, otherLambda.Invokable);
      }

      public void Capture()
      {
         var parameters = invokable.Parameters;
         foreach (var parameter in parameters.GetCapturingParameters())
         {
            var fieldName = parameter.Name;
            if (Machine.Current.Find(fieldName, true).If(out var field))
            {
               if (!fields.ContainsKey(fieldName))
               {
	               fields.New(fieldName, parameter.Mutable);
               }

               var value = field.Value;
               if (parameter.TypeConstraint.If(out var typeConstraint) && !typeConstraint.Matches(classOf(value)))
               {
	               throw incompatibleClasses(value, typeConstraint.AsString);
               }

               fields.Assign(fieldName, value, true).Force();
               providesFields = true;
            }
         }
      }
   }
}