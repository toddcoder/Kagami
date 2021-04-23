using System.IO;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Core.Monads;

namespace Kagami.Library.Nodes.Statements
{
   public class AssignToFieldWithBlock : Statement
   {
      protected bool isNew;
      protected bool mutable;
      protected string fieldName;
      protected IMaybe<TypeConstraint> typeConstraint;
      protected Block block;

      public AssignToFieldWithBlock(bool isNew, bool mutable, string fieldName, IMaybe<TypeConstraint> typeConstraint, Block block)
      {
         this.isNew = isNew;
         this.mutable = mutable;
         this.fieldName = fieldName;
         this.typeConstraint = typeConstraint;
         this.block = block;
      }

      public override void Generate(OperationsBuilder builder)
      {
         if (isNew)
         {
            builder.NewField(fieldName, mutable, true, typeConstraint);
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
         if (typeConstraint.If(out var tc))
         {
            writer.Write($"{tc.AsString} ");
         }

         writer.Write("=");
         writer.WriteLine(block);

         return writer.ToString();
      }
   }
}