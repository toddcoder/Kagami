using System;
using Kagami.Library.Invokables;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public class Lambda : IObject, IEquatable<Lambda>, IInvokableObject, ICopyFields, IPristineCopy, IProvidesFields
   {
      IInvokable invokable;
      Fields fields;
      bool providesFields;

      public Lambda(IInvokable invokable)
      {
         this.invokable = invokable;
         fields = new Fields();
         providesFields = false;
      }

      public string ClassName => "Lambda";

      public string AsString => invokable.ToString();

      public string Image => invokable.Image;

      public int Hash => invokable.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Lambda l && invokable.Index == l.invokable.Index;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => true;

      public bool Equals(Lambda other) => IsEqualTo(other);

      public override bool Equals(object obj) => Equals((Lambda)obj);

      public override int GetHashCode() => Hash;

      public IObject Copy() => new Lambda(invokable);

      public void CopyFields(Fields fields)
      {
         this.fields.CopyFrom(fields);
         providesFields = true;
      }

      public IInvokable Invokable => invokable;

      public IObject Invoke(params IObject[] arguments)
      {
         return Machine.Current.Invoke(invokable, new Arguments(arguments), fields, 0).Value;
      }

      public bool ProvidesFields => providesFields;

      public Fields Fields => fields;

      public Lambda Clone() => new Lambda(invokable);

      public IObject Join(Lambda otherLambda)
      {
         return new RuntimeFunction(args => otherLambda.Invoke(Invoke(args)), $"{Image} >> {otherLambda.Image}");
      }
   }
}