using Kagami.Library.Invokables;
using Kagami.Library.Runtime;
using Core.Collections;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class Lambda : IObject, IEquatable<Lambda>, IInvokableObject, ICopyFields, IPristineCopy, IProvidesFields
{
   protected IInvokable invokable1;
   protected Fields fields;
   protected bool providesFields;

   public Lambda(IInvokable invokable1)
   {
      this.invokable1 = invokable1;
      fields = new Fields();
      providesFields = false;
   }

   public string ClassName => "Lambda";

   public virtual string AsString => invokable1.ToString() ?? "";

   public virtual string Image => invokable1.Image;

   public virtual int Hash => invokable1.GetHashCode();

   public virtual bool IsEqualTo(IObject obj) => obj is Lambda l && invokable1.Index == l.invokable1.Index;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => true;

   public bool Equals(Lambda? other) => IsEqualTo(other!);

   public override bool Equals(object? obj) => Equals((Lambda)obj!);

   public override int GetHashCode() => Hash;

   public virtual IObject Copy() => new Lambda(invokable1);

   public void CopyFields(Fields fields)
   {
      this.fields.CopyFrom(fields);
      providesFields = true;
   }

   public IInvokable Invokable => invokable1;

   public virtual IObject Invoke(params IObject[] arguments)
   {
      var _value = Machine.Current.Value.Invoke(invokable1, new Arguments(arguments), fields, 0);
      if (_value is (true, var value))
      {
         return value;
      }
      else if (_value.Exception is (true, var exception))
      {
         throw exception;
      }
      else
      {
         throw fieldNotFound(invokable1.Image);
      }
   }

   public bool ProvidesFields => providesFields;

   public Fields Fields => fields;

   public Lambda Clone() => new(invokable1);

   public IObject Join(Lambda otherLambda) => new CompositeLambda(invokable1, otherLambda.Invokable);

   public void Capture()
   {
      var parameters = invokable1.Parameters;
      foreach (var parameter in parameters.GetCapturingParameters())
      {
         var fieldName = parameter.Name;
         if (Machine.Current.Value.Find(fieldName, true) is (true, var field))
         {
            if (!fields.ContainsKey(fieldName))
            {
               fields.New(fieldName, parameter.Mutable);
            }

            var value = field.Value;
            if (parameter.TypeConstraint is (true, var typeConstraint) && !typeConstraint.Matches(classOf(value)))
            {
               throw incompatibleClasses(value, typeConstraint.AsString);
            }

            fields.Assign(fieldName, value, true).Force();
            providesFields = true;
         }
      }
   }

   public Int ParameterCount => invokable1.Parameters.Length;
}