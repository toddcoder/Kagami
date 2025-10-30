using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class TypedArray : KArray
{
   protected TypeConstraint typeConstraint;

   public static IObject CreateObject(IEnumerable<IObject> objects, TypeConstraint typeConstraint) => new TypedArray(objects, typeConstraint);

   public TypedArray(IEnumerable<IObject> objects, TypeConstraint typeConstraint) : base(objects)
   {
      this.typeConstraint = typeConstraint;
      foreach (var item in list)
      {
         assertMatchesType(typeConstraint, item);
      }
   }

   public TypedArray(IObject value, TypeConstraint typeConstraint) : base(value)
   {
      this.typeConstraint = typeConstraint;
      foreach (var item in list)
      {
         assertMatchesType(typeConstraint, item);
      }
   }

   public override IObject this[int index]
   {
      get => base[index];
      set
      {
         assertMatchesType(typeConstraint, value);
         base[index] = value;
      }
   }

   public override IObject this[Sequence sequence]
   {
      get => base[sequence];
      set
      {
         foreach (var item in sequence.List)
         {
            assertMatchesType(typeConstraint, item);
         }

         base[sequence] = value;
      }
   }

   public override IObject Set(IObject index, IObject value)
   {
      assertMatchesType(typeConstraint, value);
      return base.Set(index, value);
   }

   public override KBoolean In(IObject item)
   {
      assertMatchesType(typeConstraint, item);
      return base.In(item);
   }

   public override KBoolean NotIn(IObject item)
   {
      assertMatchesType(typeConstraint, item);
      return base.NotIn(item);
   }

   public override IObject Append(IObject obj)
   {
      assertMatchesType(typeConstraint, obj);
      return base.Append(obj);
   }

   public override IObject Remove(IObject obj)
   {
      assertMatchesType(typeConstraint, obj);
      return base.Remove(obj);
   }

   public override IObject InsertAt(int index, IObject obj)
   {
      assertMatchesType(typeConstraint, obj);
      return base.InsertAt(index, obj);
   }

   public override IObject Assign(SkipTake skipTake, IEnumerable<IObject> values)
   {
      IObject[] enumerable = [.. values];
      foreach (var value in enumerable)
      {
         assertMatchesType(typeConstraint, value);
      }

      return base.Assign(skipTake, enumerable);
   }

   public override IObject Concatenate(KArray kArray)
   {
      if (kArray is not TypedArray typedArray)
      {
         throw fail("Typed Array required");
      }

      if (!typedArray.typeConstraint.IsEquivalentTo(typeConstraint))
      {
         throw fail("Types don't match");
      }

      return base.Concatenate(kArray);
   }

   public override IObject IndexOf(IObject item)
   {
      assertMatchesType(typeConstraint, item);
      return base.IndexOf(item);
   }

   public override IObject LastIndexOf(IObject item)
   {
      assertMatchesType(typeConstraint, item);
      return base.LastIndexOf(item);
   }

   public override IObject BinarySearch(IObject item)
   {
      assertMatchesType(typeConstraint, item);
      return base.BinarySearch(item);
   }

   public override IObject BinarySearch(IObject item, Lambda lambda)
   {
      assertMatchesType(typeConstraint, item);
      return base.BinarySearch(item, lambda);
   }

   public override IObject Prepend(IObject item)
   {
      assertMatchesType(typeConstraint, item);
      return base.Prepend(item);
   }

   public override KArray PadLeft(int count, IObject value)
   {
      assertMatchesType(typeConstraint, value);
      return base.PadLeft(count, value);
   }

   public override KArray PadRight(int count, IObject value)
   {
      assertMatchesType(typeConstraint, value);
      return base.PadRight(count, value);
   }

   public override IObject Accept(IObject obj)
   {
      assertMatchesType(typeConstraint, obj);
      return base.Accept(obj);
   }
}