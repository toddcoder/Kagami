using System.IO;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Core.Monads;

namespace Kagami.Library.Nodes.Statements;

public class AssignToFieldWithBlock : Statement
{
   protected bool isNew;
   protected bool mutable;
   protected string fieldName;
   protected Maybe<TypeConstraint> _typeConstraint;
   protected Block block;

   public AssignToFieldWithBlock(bool isNew, bool mutable, string fieldName, Maybe<TypeConstraint> _typeConstraint, Block block)
   {
      this.isNew = isNew;
      this.mutable = mutable;
      this.fieldName = fieldName;
      this._typeConstraint = _typeConstraint;
      this.block = block;
   }

   public override void Generate(OperationsBuilder builder)
   {
      if (isNew)
      {
         builder.NewField(fieldName, mutable, true, _typeConstraint);
      }

      builder.PushFrame();
      block.Generate(builder);
      builder.PopFrameWithValue();
      builder.Peek(Index);
      builder.AssignField(fieldName, true);
   }

   public override string ToString()
   {
      using var writer = new StringWriter();
      if (!isNew)
      {
         writer.Write(mutable ? "var " : "let ");
      }

      writer.Write($"{fieldName} ");
      if (_typeConstraint is (true, var typeConstraint))
      {
         writer.Write($"{typeConstraint.AsString} ");
      }

      writer.Write("=");
      writer.WriteLine(block);

      return writer.ToString();
   }
}