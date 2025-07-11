using Kagami.Library.Invokables;
using Kagami.Library.Runtime;
using Core.Collections;
using Kagami.Library.Packages;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class Lambda : IObject, IEquatable<Lambda>, IInvokableObject, ICopyFields, IPristineCopy, IProvidesFields
{
   protected IInvokable invokable1;
   protected Fields fields = new();
   protected bool providesFields;
   protected bool captures;

   public Lambda(IInvokable invokable1, bool captures)
   {
      this.invokable1 = invokable1;
      this.captures = captures;
   }

   public string ClassName => "Lambda";

   public virtual string AsString => invokable1.ToString() ?? "";

   public virtual string Image => $"{invokable1.Image}";

   public virtual int Hash => invokable1.GetHashCode();

   public virtual bool IsEqualTo(IObject obj) => obj is Lambda l && invokable1.Index == l.invokable1.Index;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();

   public bool Equals(Lambda? other) => IsEqualTo(other!);

   public override bool Equals(object? obj) => Equals((Lambda)obj!);

   public override int GetHashCode() => Hash;

   public virtual IObject Copy() => new Lambda(invokable1, captures);

   public void CopyFields(Fields fields)
   {
      this.fields.CopyFrom(fields);
      providesFields = true;
   }

   public IInvokable Invokable => invokable1;

   public virtual IObject Invoke(params IObject[] arguments)
   {
      var machine = Machine.Current.Value;
      var _value = machine.Invoke(invokable1, new Arguments(arguments), fields);
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
         return KVoid.Value;
      }
   }

   public bool ProvidesFields => providesFields;

   protected Fields getFields()
   {
      var newFields = new Fields();
      foreach (var (fieldName, field) in fields)
      {
         var newField = new Field
         {
            Visible = field.Visible,
            Mutable = field.Mutable,
            Value = field.Value,
            TypeConstraint = field.TypeConstraint,
            Tolerant = true
         };
         newFields.New(fieldName, newField);
      }

      return newFields;
   }

   public Fields Fields => getFields();

   public Lambda Clone() => new(invokable1, captures);

   public IObject Join(Lambda otherLambda) => new CompositeLambda(invokable1, otherLambda.Invokable);

   public void Capture(Machine machine)
   {
      if (!captures)
      {
         return;
      }

      var frames = machine.PeekFramesUntil(f => f.FrameType == FrameType.Function);
      foreach (var field in frames.AllFields().Where(f => f.field.Value.Id != Id && noForbidden(f.field.Value)))
      {
         if (!fields.ContainsKey(field.fieldName))
         {
            fields.AssignLocal(field.fieldName, field.field.Value, true).Force();
         }
      }

      return;

      bool noForbidden(IObject value) => value is not Package && value is not PackageFunction && value is not PackageClass;
   }

   public Int ParameterCount => invokable1.Parameters.Length;
}