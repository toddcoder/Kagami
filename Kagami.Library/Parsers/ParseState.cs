using Core.Collections;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Parsers.Expressions;
using System.Collections;
using System.Text.RegularExpressions;
using Core.DataStructures;
using Core.Numbers;
using Kagami.Library.Invokables;
using Kagami.Library.Operations;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;
using DefineNewField = Kagami.Library.Nodes.Statements.DefineNewField;
using Group = System.Text.RegularExpressions.Group;

namespace Kagami.Library.Parsers;

public class ParseState : IEnumerable<Statement>
{
   protected string source;
   protected int index;
   protected int line;
   protected int character;
   protected List<Statement> statements = [];
   protected Stack<List<Statement>> statementStack = new();
   protected List<Token> tokens = [];
   protected IndexTransaction indexTransaction = new();
   protected TokenTransaction tokenTransaction;
   protected List<Symbol> postGenerationSymbols = [];
   protected Maybe<int> _exceptionIndex = nil;
   protected Stack<bool> yieldingStack = new();
   protected MaybeStack<Maybe<TypeConstraint>> returnTypesStack = new();
   protected Hash<string, Expression> defExpressions = [];
   protected Hash<string, Function> macros = [];
   protected Stack<Maybe<IPrefixCode>> prefixCodes = new();
   protected Stack<Maybe<ImplicitState>> implicitStates = new();
   protected Stack<ImplicitExpressionState> implicitExpressionStates = new();
   protected StringSet patterns = [];
   protected Maybe<Exception> _exception = nil;
   protected StringHash<Parameter> parameters = [];
   protected MaybeStack<InvokeSymbol> annotations = [];

   public ParseState(string source)
   {
      this.source = source.TrimEnd();
      tokenTransaction = new TokenTransaction(tokens);
   }

   public bool AllowPrintStatement { get; set; }

   public bool AllowReorder { get; set; } = true;

   public Maybe<int> ExceptionIndex
   {
      get => _exceptionIndex;
      set => _exceptionIndex = value;
   }

   public void SetExceptionIndex() => _exceptionIndex = index.Some();

   public Statement[] Statements() => [.. statements];

   public Maybe<Statement> PopStatement()
   {
      if (statements.Count > 0)
      {
         var lastStatement = statements[^1];
         statements.RemoveAt(statements.Count - 1);

         return lastStatement;
      }
      else
      {
         return nil;
      }
   }

   public void BeginTransaction()
   {
      indexTransaction.Begin(index);
      tokenTransaction.Begin();
   }

   public void RollBackTransaction()
   {
      index = indexTransaction.RollBack();
      tokenTransaction.RollBack();
   }

   public void CommitTransaction()
   {
      indexTransaction.Commit();
      tokenTransaction.Commit();
   }

   public Optional<Unit> RollBackTransactionIf<T>(bool enabled, Optional<T> _previousMatch) where T : notnull
   {
      if (enabled && !_previousMatch)
      {
         RollBackTransaction();
      }

      return _previousMatch.Map(_ => unit);
   }

   public void PushStatements()
   {
      statementStack.Push(statements);
      statements = [];
   }

   public Result<List<Statement>> PopStatements() => tryTo(() =>
   {
      var returnStatements = statements;
      statements = [..statementStack.Pop()];
      return returnStatements;
   });

   public bool More => index < source.Length;

   public void AddStatement(Statement statement)
   {
      statement.Index = index;

      if (ForExpression is (true, var (fieldName, expression)))
      {
         statements.Add(new For(new PlaceholderSymbol("+" + fieldName), expression, new Block(statement), new PossibleIfExpression.None()));
         ForExpression = nil;
      }
      else
      {
         statements.Add(statement);
      }

      character += statement.Length;
   }

   public void AddToken(int index, int length, Color color, string text = "")
   {
      var token = new Token(index, length, text);
      setTokenColor(token, color);

      tokens.Add(token);
   }

   public void AddToken(Color color, int length = 1, string text = "") => AddToken(index, length, color, text);

   public Optional<Unit> BeginBlock(Color beginBlockColor = Color.Block)
   {
      return Scan(@"^(\s*)(\{)(?!\|)", Color.Whitespace, beginBlockColor).Map(_ => unit);
   }

   public Optional<Unit> EndBlock(Color endBlockColor = Color.Block)
   {
      return Scan(@"^(\s*)(\})", Color.Whitespace, endBlockColor).Map(_ => unit);
   }

   public bool PeekEndBlock() => CurrentSource.IsMatch(@"^\s*\}");

   public int Index => index;

   public string Source => source;

   public string CurrentSource => source.Drop(index);

   public IEnumerator<Statement> GetEnumerator() => statements.GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public void Move(int length) => index += length;

   public void Move(Token[] tokens)
   {
      if (tokens.Length != 0)
      {
         Move(tokens[0].Length);
      }
   }

   public Token[] Tokens => [.. tokens];

   protected void setTokenColor(Token token, Color color) => token.Color = token.Text.Trim() switch
   {
      "(" => token.Length == 0 ? Color.Structure : color,
      "[" => token.Length == 0 ? Color.Structure : color,
      ")" => token.Length == 0 ? Color.Structure : color,
      "]" => token.Length == 0 ? Color.Structure : color,
      "," => Color.Structure,
      _ => color
   };

   public Optional<string> Scan(string pattern, params Color[] colors) => Scan(pattern, RegexOptions.None, colors);

   public Optional<string> Scan(string pattern, RegexOptions options, params Color[] colors)
   {
      return Scan(pattern, 0, options, colors);
   }

   public Optional<string> Scan(string pattern, int groupIndex, params Color[] colors)
   {
      return Scan(pattern, groupIndex, RegexOptions.Compiled, colors);
   }

   public Optional<string> Scan(string pattern, int groupIndex, RegexOptions options, params Color[] colors)
   {
      try
      {
         var regex = new System.Text.RegularExpressions.Regex(pattern, options);
         var matches = regex.Matches(CurrentSource);
         if (matches.Count > 0)
         {
            var match = matches[0];
            Group[] groupArray = [.. match.AllGroups().Skip(1).Take(match.Groups.Count - 1)];
            for (var i = 0; i < Math.Min(groupArray.Length, colors.Length); i++)
            {
               var length = groupArray[i].Length;
               AddToken(colors[i], length, groupArray[i].Value);
               Move(length);
            }

            return match.Groups[groupIndex].Value;
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public bool LookAhead(string pattern, RegexOptions options)
   {
      var regex = new System.Text.RegularExpressions.Regex(pattern, options);
      return regex.IsMatch(CurrentSource);
   }

   public bool LookAhead(string pattern) => LookAhead(pattern, RegexOptions.None);

   public Maybe<string> LookAhead(string pattern, RegexOptions options, int groupIndex)
   {
      var regex = new System.Text.RegularExpressions.Regex(pattern, options);
      var match = regex.Match(CurrentSource);
      if (match.Success)
      {
         return match.Groups[groupIndex].Value;
      }
      else
      {
         return nil;
      }
   }

   public Maybe<string> LookAhead(string pattern, int groupIndex) => LookAhead(pattern, RegexOptions.None, groupIndex);

   public Optional<OptionalScanResult> OptionalScan(string pattern, params Color[] colors) => OptionalScan(pattern, RegexOptions.None, colors);

   public Optional<OptionalScanResult> OptionalScan(string pattern, RegexOptions options, params Color[] colors)
   {
      try
      {
         var regex = new System.Text.RegularExpressions.Regex(pattern, options);
         var matches = regex.Matches(CurrentSource);
         if (matches.Count > 0)
         {
            var match = matches[0];
            Group[] groupArray = [.. match.AllGroups().Skip(1).Take(match.Groups.Count - 1)];
            for (var i = 0; i < Math.Min(groupArray.Length, colors.Length); i++)
            {
               var length = groupArray[i].Length;
               AddToken(colors[i], length, groupArray[i].Value);
               Move(length);
            }

            return new OptionalScanResult.Value(match.Value);
         }
         else
         {
            return new OptionalScanResult.NoValue();
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Optional<string> Scan(string pattern, Func<Group, int, Color> colorFunc) => Scan(pattern, RegexOptions.None, colorFunc);

   public Optional<string> Scan(string pattern, RegexOptions options, Func<Group, int, Color> colorFunc)
   {
      try
      {
         var regex = new System.Text.RegularExpressions.Regex(pattern, options);
         var matches = regex.Matches(CurrentSource);
         if (matches.Count > 0)
         {
            var match = matches[0];
            Group[] groupArray = [.. match.AllGroups().Skip(1).Take(match.Groups.Count - 1)];
            var groupIndex = 1;
            foreach (var group in groupArray)
            {
               var length = group.Length;
               AddToken(colorFunc(group, groupIndex++), length, group.Value);
               Move(length);
            }

            return match.Value;
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Optional<string> SkipEndOfLine()
   {
      var _skipEndOfLine = Scan("(^\r\n|^\r|^\n)", Color.Whitespace);
      if (_skipEndOfLine)
      {
         line++;
         character = 0;
      }

      return _skipEndOfLine;
   }

   public void Colorize(Token[] tokens, params Color[] colors)
   {
      for (var i = 0; i < colors.Length; i++)
      {
         setTokenColor(tokens[i + 1], colors[i]);
      }

      foreach (var token in tokens.Skip(1))
      {
         this.tokens.Add(token);
      }

      Move(tokens);
   }

   public void Colorize(int startIndex, string text, Color color)
   {
      var length = text.Length;
      var token = new Token(startIndex, length, text);
      setTokenColor(token, color);
      tokens.Add(token);
   }

   public void Colorize(int startIndex, char @char, Color color)
   {
      var text = @char.ToString();
      Colorize(startIndex, text, color);
   }

   public void AddSymbol(Symbol symbol) => postGenerationSymbols.Add(symbol);

   public List<Symbol> PostGenerationSymbols => postGenerationSymbols;

   public void UpdateStatement(int index, int length)
   {
      var count = statements.Count;

      if (count > 0)
      {
         var statement = statements[count - 1];
         statement.Index = index;
         statement.Length = length;
      }
   }

   public Statement LastStatement => statements[^1];

   public Optional<Maybe<Expression>> getAnd()
   {
      var builder = new ExpressionBuilder(ExpressionFlags.OmitIf);
      var parser = new IfAsAndParser(builder);
      var _result = parser.Scan(this);
      if (_result)
      {
         var _expression = builder.ToExpression();
         if (_expression is (true, var expression))
         {
            return expression.Some();
         }
         else
         {
            return _expression.Exception;
         }
      }
      else if (_result.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return nil;
      }
   }

   public void CreateYieldFlag() => yieldingStack.Push(false);

   public void CreateReturnType() => returnTypesStack.Push(nil);

   public void SetYieldFlag()
   {
      yieldingStack.Pop();
      yieldingStack.Push(true);
   }

   public void SetReturnType(Maybe<TypeConstraint> typeConstraint)
   {
      returnTypesStack.Pop();
      returnTypesStack.Push(typeConstraint);
   }

   public bool RemoveYieldFlag() => yieldingStack.Pop();

   public Maybe<TypeConstraint> GetReturnType() => returnTypesStack.Peek().Map(p => p);

   public void RemoveReturnType() => returnTypesStack.Pop();

   public void RegisterDefExpression(string fieldName, Expression expression) => defExpressions[fieldName] = expression;

   public Maybe<Expression> DefExpression(string fieldName) => defExpressions.Maybe[fieldName];

   public void RegisterMacro(Function function) => macros[function.Selector.Name] = function;

   public Maybe<Function> Macro(string fullFunctionName) => macros.Maybe[fullFunctionName];

   public bool BlockFollows() => CurrentSource.IsMatch("^ /s* '{'; m");

   public Maybe<(string, Expression)> ForExpression { get; set; } = nil;

   public Maybe<(string, Symbol)> LeftZipExpression { get; set; } = nil;

   public Maybe<(string, Symbol)> RightZipExpression { get; set; } = nil;

   public void BeginPrefixCode() => prefixCodes.Push(nil);

   public Maybe<IPrefixCode> PrefixCode
   {
      get => prefixCodes.Peek();
      set
      {
         _ = prefixCodes.Pop();
         prefixCodes.Push(value);
      }
   }

   public void EndPrefixCode() => prefixCodes.Pop();

   public void BeginImplicitState() => implicitStates.Push(nil);

   public Maybe<ImplicitState> ImplicitState
   {
      get => implicitStates.Peek();
      set
      {
         _ = implicitStates.Pop();
         implicitStates.Push(value);
      }
   }

   public void EndImplicitState() => implicitStates.Pop();

   public void BeginImplicitExpressionState() => implicitExpressionStates.Push(new ImplicitExpressionState());

   public ImplicitExpressionState ImplicitExpressionState => implicitExpressionStates.Peek();

   public void EndImplicitExpressionState() => implicitExpressionStates.Pop();

   public void RegisterPattern(string patternName) => patterns.Add(patternName);

   public bool IsPattern(string patternName) => patterns.Contains(patternName);

   public Optional<string> ScanFormat()
   {
      var _result = Scan(@"^(\[)([cdefgnprxsboi](?:-?\d+)?(?:\.\d+)?)(\])", Color.Format, Color.Format, Color.Format);
      if (_result)
      {
         return _result.Map(r => r.Drop(1).Drop(-1));
      }
      else
      {
         _result = Scan(@"^(\[)([^\]]+)(\])", Color.Format, Color.Format, Color.Format);
         _result = _result.Map(r => r.Drop(1).Drop(-1));
      }

      return _result;
   }

   public bool NotKeyword() => Scan(@"^(\s*)(not)\b", Color.Whitespace, Color.Keyword).Map(n => n.IsNotEmpty()) | false;

   public Maybe<Symbol> StartingValueSymbol { get; set; } = nil;

   public int Line => line;

   public int Character => character;

   public int LastTokenIndex => tokens.Count - 1;

   public Maybe<Color> this[int index]
   {
      get
      {
         if (index.Between(0).Until(tokens.Count))
         {
            return tokens[index].Color;
         }
         else
         {
            return nil;
         }
      }
      set
      {
         if (value is (true, var color) && index.Between(0).Until(tokens.Count))
         {
            tokens[index].Color = color;
         }
      }
   }

   public Exception SetException(string message, Exception caughtException)
   {
      if (!_exception)
      {
         _exception = new Exception(message, caughtException);
      }

      return caughtException;
   }

   public Maybe<Exception> SetException(string message, Maybe<Exception> _caughtException)
   {
      if (_caughtException is (true, var caughtException))
      {
         SetException(message, caughtException);
      }
      else
      {
         SetException(message);
      }

      return _caughtException;
   }

   public Maybe<Exception> SetException(string message)
   {
      _exception = new Exception(message);
      return _exception;
   }

   public Maybe<Exception> Exception => _exception;

   public void ClearParameters() => parameters.Clear();

   public void RegisterParameter(DefineNewField defineNewField)
   {
      var (mutable, fieldName, className, isHidden, isParam) = defineNewField;
      if (isParam)
      {
         var typeConstraint = TypeConstraint.FromList(className);
         parameters[fieldName] = new Parameter(isHidden, mutable, fieldName, fieldName, nil, typeConstraint, false, false, false);
      }
   }

   public void RegisterParameter(AssignToNewField assignToNewField)
   {
      var (mutable, fieldName, _typeConstraint, isHidden, expression) = assignToNewField;
      var symbol = new InvokableExpressionSymbol(expression);
      AddSymbol(symbol);
      PossibleTypeConstraint possibleTypeConstraint;
      if (_typeConstraint is (true, var typeConstraint))
      {
         possibleTypeConstraint = new PossibleTypeConstraint.Some(typeConstraint);
      }
      else
      {
         possibleTypeConstraint = new PossibleTypeConstraint.None();
      }

      var possibleInvokable = new PossibleInvokable.Some(symbol.Invokable);
      parameters[fieldName] = new Parameter(isHidden, mutable, fieldName, fieldName, possibleInvokable, possibleTypeConstraint, false, false, false);
   }

   public StringHash<Parameter> Parameters => parameters;

   public void PushAnnotation(InvokeSymbol invokeSymbol) => annotations.Push(invokeSymbol);

   public Maybe<InvokeSymbol> PopAnnotation() => annotations.Pop();

   public void ProcessAnnotations(IAnnotatable annotatable, OperationsBuilder builder)
   {
      if (annotations.IsNotEmpty)
      {
         var selector = annotatable.Selector;
         var lambda = annotatable.Lambda;
         while (annotations.Pop() is (true, var invokeSymbol))
         {
            Expression[] implicitArguments = [Expression.FromSymbol(new SelectorSymbol(selector)), Expression.FromSymbol(new PushSymbol(lambda))];
            Expression[] newArguments = [.. implicitArguments, .. invokeSymbol.Arguments];
            var newInvokeSymbol = new InvokeSymbol(invokeSymbol.FunctionName, newArguments, invokeSymbol.Lambda, invokeSymbol.InComparisand);
            newInvokeSymbol.Generate(builder);
         }
      }
   }
}