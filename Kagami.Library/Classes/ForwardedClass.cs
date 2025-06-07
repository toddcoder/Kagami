namespace Kagami.Library.Classes;

public class ForwardedClass : BaseClass
{
   public ForwardedClass(string name) => Name = name;

   public override string Name { get; }
}