using System.Text.RegularExpressions;
using Core.Enumerables;
using Kagami.Library.Runtime;
using Core.Monads;
using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class MixinNameParser : PatternedParser
{
   protected List<Mixin> mixins;

   public MixinNameParser(List<Mixin> mixins) : base(true)
   {
      this.mixins = mixins;
   }

   //public override string Pattern => $"^ /(/s*) /({REGEX_CLASS}) (/(/s*) /',')?";

   [GeneratedRegex(@$"^(\s*)({REGEX_CLASS})(?:(\s*)(,))?")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens)
   {
      var mixinName = tokens[2].Text;
      var more = tokens[4].Text == ",";
      state.Colorize(tokens, Color.Whitespace, Color.Class, Color.Whitespace, Color.Structure);

      var _mixin = Module.Global.Value.Mixin(mixinName);
      if (_mixin is (true, var mixin))
      {
         if (!mixins.FirstOrNone())
         {
            mixins.Add(mixin);
         }

         More = more;
         return unit;
      }
      else
      {
         return mixinNotFound(mixinName);
      }
   }

   public bool More { get; set; }
}