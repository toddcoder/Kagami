namespace Kagami.Library.Objects;

public interface IMessageNumber
{
   IObject Negate();

   IObject Sign();

   IObject Raise(INumeric other);

   IObject Remainder(INumeric other);

   IObject Divide(INumeric other);

   IObject DivRem(INumeric other);

   IObject Add(INumeric other);

   IObject Subtract(INumeric other);

   IObject Multiply(INumeric other);

   IObject Sin();

   IObject Cos();

   IObject Tan();

   IObject Asin();

   IObject Acos();

   IObject Atan();

   IObject Atan2(INumeric other);

   IObject Sinh();

   IObject Cosh();

   IObject Tanh();

   IObject Asinh();

   IObject Acosh();

   IObject Atanh();

   IObject Sqrt();

   IObject Log();

   IObject Ln();

   IObject Exp();

   IObject Abs();

   IObject Ceiling();

   IObject Floor();

   IObject Fraction();

   IObject Round(INumeric other);
}