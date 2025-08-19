using Core.Collections;
using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Packages.Matching;

public abstract class Matcher : IObject
{
   protected int index = -1;
   protected int length = 0;
   protected Maybe<Matcher> _next = nil;
   protected Maybe<Matcher> _alternate = nil;
   protected Maybe<Matcher> _lastNext = nil;
   protected Maybe<Matcher> _lastAlternate = nil;
   //protected Maybe<Replacement> _replacement = nil;

   public int Index => index;

   public int Length => length;

   /*public virtual Maybe<Replacement> Replacement
   {
      get => _replacement;
      set => _replacement = value;
   }*/

   public virtual Maybe<Matcher> Next
   {
      get => _next;
      set => _next = value;
   }

   public virtual Maybe<Matcher> Alternate
   {
      get => _alternate;
      set => _alternate = value;
   }

   public virtual void AppendNext(Matcher matcher)
   {
      if (_lastNext is (true, var lastNext))
      {
         lastNext.Next = matcher;
      }
      else
      {
         _next = matcher;
      }

      _lastNext = matcher;
   }

   public virtual void AppendAlternate(Matcher matcher)
   {
      if (_lastAlternate is (true, var lastAlternate))
      {
         lastAlternate.Alternate = matcher;
      }
      else
      {
         _alternate = matcher;
      }

      _lastAlternate = matcher;
   }

   public virtual bool PositionAlreadyUpdated => false;

   public abstract Matcher Clone();

   protected Maybe<Matcher> cloneNext() => _next.Map(m => m.Clone());

   protected Maybe<Matcher> cloneAlternate() => _alternate.Map(m => m.Clone());

   //protected Maybe<Replacement> cloneReplacement() => _replacement.Map(m => m.Clone());

   public virtual void Initialize()
   {
   }

   public Maybe<Lambda> Predicate { get; set; } = nil;

   public bool IsValid => Predicate.Map(l => l.Invoke(this).IsTrue) | false;

   public virtual bool Failed => false;

   public virtual bool Aborted => false;

   protected Matcher clone(Matcher matcher)
   {
      matcher.Next = cloneNext();
      matcher.Alternate = cloneAlternate();
      //matcher.Replacement = cloneReplacement();

      return matcher;
   }

   public virtual bool AutoOptional => false;

   public abstract string ClassName { get; }

   public abstract string AsString { get; }

   public abstract string Image { get; }

   public abstract int Hash { get; }

   public abstract bool IsEqualTo(IObject obj);

   public abstract bool Match(IObject comparisand, Hash<string, IObject> bindings);

   public abstract bool IsTrue { get; }

   public Guid Id { get; init; } = Guid.NewGuid();
}