namespace Kagami.Library.Operations
{
   public class Parameter
   {
      public Parameter(string name, bool mutable)
      {
         Name = name;
         Mutable = mutable;
      }

      public string Name { get; }

      public bool Mutable { get; }

      public void Deconstruct(out string name, out bool mutable)
      {
         name = Name;
         mutable = Mutable;
      }

      public override string ToString() => $"{(Mutable ? "var " : "")} {Name}";
   }
}