namespace Kagami.Library.Objects
{
   public interface IMayInvoke
   {
      IObject Invoke(params IObject[] arguments);
   }
}