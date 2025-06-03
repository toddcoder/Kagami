namespace Kagami.Library.Operations;

public abstract class AddressedOperation : Operation, INonAdvancing
{
   protected int address;
   protected bool increment;

   public AddressedOperation()
   {
      address = -1;
      increment = false;
   }

   public override bool Increment => increment;

   public int Address
   {
      get => address;
      set => address = value;
   }
}