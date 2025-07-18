using Kagami.Library.Invokables;
using Kagami.Library.Runtime;
using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class Lambda : IObject, IEquatable<Lambda>, IInvokableObject, ICopyFields, IPristineCopy, IProvidesFields
{
   protected IInvokable invokable;
   protected Fields fields = new();
   protected bool providesFields;
   protected bool captures;

   public Lambda(IInvokable invokable, bool captures)
   {
      this.invokable = invokable;
      this.captures = captures;
   }

   public string ClassName => "Lambda";

   public virtual string AsString => invokable.ToString() ?? "";

   public virtual string Image => $"{invokable.Image}";

   public virtual int Hash => invokable.GetHashCode();

   public virtual bool IsEqualTo(IObject obj) => obj is Lambda l && invokable.Index == l.invokable.Index;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();

   public bool Equals(Lambda? other) => IsEqualTo(other!);

   public override bool Equals(object? obj) => Equals((Lambda)obj!);

   public override int GetHashCode() => Hash;

   public virtual IObject Copy() => new Lambda(invokable, captures);

   public void CopyFields(Fields fields)
   {
      this.fields.CopyFrom(fields);
      foreach (var parameter in invokable.Parameters)
      {
         if (this.fields.ContainsKey(parameter.Name))
         {
            this.fields.Remove(parameter.Name);
         }
      }

      providesFields = true;
   }

   public IInvokable Invokable => invokable;

   public virtual IObject Invoke(params IObject[] arguments)
   {
      var machine = Machine.Current.Value;
      var _value = machine.Invoke(invokable, new Arguments(arguments), fields, true);
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
            Tolerant = true,
            Type = field.Type
         };
         newFields.New(fieldName, newField);
      }

      return newFields;
   }

   public Fields Fields => getFields();

   public Lambda Clone() => new(invokable, captures);

   public IObject Join(Lambda otherLambda) => new CompositeLambda(invokable, otherLambda.Invokable);

   public void Capture(Machine machine)
   {
      if (!captures)
      {
         return;
      }

      Set<string> capturing = [..invokable.Parameters.GetNoCapturingParameters().Select(p => p.Name)];

      var frames = machine.PeekFramesUntil(f => f.FrameType == FrameType.Function);
      foreach (var field in frames.AllFields().Where(f => f.field.Value.Id != Id && include(f.fieldName, f.field)))
      {
         if (!fields.ContainsKey(field.fieldName))
         {
            fields.AssignLocal(field.fieldName, FieldType.Capture, field.field.Value, true).Force();
         }
      }

      return;

      bool include(string fieldName, Field field) => field.Type is FieldType.Assignment && !capturing.Contains(fieldName);
   }

   public Int ParameterCount => invokable.Parameters.Length;

   public KTuple FieldsInTuple
   {
      get
      {
         List<NameValue> list = [];
         foreach (var (fieldName, @field) in fields)
         {
            if (@field.Value is not Unassigned)
            {
               list.Add(new NameValue(fieldName, @field.Value));
            }
         }

         return new KTuple([.. list]);
      }
   }
}