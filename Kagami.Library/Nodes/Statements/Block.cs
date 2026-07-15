using System.Collections;
using Core.Collections;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Expressions;
using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Classes;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Nodes.Statements;

public class Block : Statement, IEnumerable<Statement>
{
   public static Block Getter(string fieldName, Maybe<TypeConstraint> _typeConstraint)
   {
      var expressionBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
      expressionBuilder.Add(new FieldSymbol(fieldName));
      var _expression = expressionBuilder.ToExpression();
      if (_expression is (true, var expression))
      {
         return new Block(new Return(expression, _typeConstraint));
      }
      else
      {
         throw _expression.Exception;
      }
   }

   public static Block Getter(string fieldName) => Getter(fieldName, nil);

   public static Block Setter(string fieldName, string parameterName, Maybe<TypeConstraint> _typeConstraint)
   {
      var expressionBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
      expressionBuilder.Add(new FieldSymbol(parameterName));
      var _expression = expressionBuilder.ToExpression();
      if (_expression is (true, var expression))
      {
         var assignToField = new AssignToField(fieldName, nil, expression);
         var setter = new Block(assignToField)
         {
            TypeConstraint = _typeConstraint
         };
         setter.AddReturnIf(new FieldSymbol(fieldName));
         return setter;
      }
      else
      {
         throw _expression.Exception;
      }
   }

   public static Block Setter(string fieldName, string parameterName) => Setter(fieldName, parameterName, nil);

   public static Block Merge(Block leftBlock, Block rightBlock)
   {
      return new Block(merge().ToList());

      IEnumerable<Statement> merge()
      {
         foreach (var statement in leftBlock)
         {
            yield return statement;
         }

         foreach (var statement in rightBlock)
         {
            yield return statement;
         }
      }
   }

   protected List<Statement> statements;
   protected Maybe<TypeConstraint> _typeConstraint;
   protected Hash<Guid, ReplacementTypeConstraint> replacementTypeConstraints = [];

   public Block(List<Statement> statements, Maybe<TypeConstraint> _typeConstraint)
   {
      this.statements = statements;
      this._typeConstraint = _typeConstraint;
   }

   public Block(List<Statement> statements)
   {
      this.statements = statements;
      _typeConstraint = nil;
   }

   public Block(Statement statement, Maybe<TypeConstraint> _typeConstraint)
   {
      statements = [statement];
      this._typeConstraint = _typeConstraint;
   }

   public Block(Statement statement)
   {
      statements = [statement];
      _typeConstraint = nil;
   }

   public Block(Expression expression) : this(new Return(expression, nil))
   {
   }

   public Block(Symbol symbol) : this(new Return(new Expression(symbol), nil))
   {
   }

   public Block()
   {
      statements = [];
      _typeConstraint = nil;
   }

   public bool Yielding { get; set; }

   public Maybe<TypeConstraint> TypeConstraint
   {
      get => _typeConstraint;
      set => _typeConstraint = value;
   }

   public void Unshift(Statement statement) => statements.Insert(0, statement);

   public override void Generate(OperationsBuilder builder)
   {
      foreach (var statement in statements)
      {
         statement.Generate(builder);
      }

      if (Yielding)
      {
         builder.PushNil();
         builder.Return(true);
      }
      else if (_typeConstraint is (true, var typeConstraint))
      {
         builder.ReturnType(true, typeConstraint);
      }
   }

   public IEnumerator<Statement> GetEnumerator() => statements.GetEnumerator();

   public override string ToString() => statements.ToString("\r\n");

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public void Add(Statement statement) => statements.Add(statement);

   public void AddReturnIf()
   {
      if (statements.Count > 0 && statements[^1] is not ReturnNothing)
      {
         statements.Add(new ReturnNothing());
      }
   }

   public void AddReturnIf(Symbol symbol)
   {
      if (statements.Count > 0 && statements[^1] is not Return)
      {
         var expression = new Expression(symbol);
         statements.Add(new Return(expression, nil));
      }
   }

   public void AddReturnUnitIf() => AddReturnIf(new PushObjectSymbol(KUnit.Value));

   public Maybe<Expression> ExpressionStatement(bool returns)
   {
      if (statements.Count > 0 && statements[0] is ExpressionStatement expressionStatement &&
          expressionStatement.ReturnExpression == returns)
      {
         return expressionStatement.Expression;
      }
      else
      {
         return nil;
      }
   }

   public Block Clone()
   {
      var block = new Block();
      foreach (var statement in statements)
      {
         block.Add(statement);
      }

      return block;
   }

   public void Insert(int index, Statement statement) => statements.Insert(index, statement);

   public void Insert(Statement statement) => statements.Insert(0, statement);

   public void InsertSelfAlias(string aliasName)
   {
      var symbol = new FieldSymbol("self");
      var expression = new Expression(symbol);
      var assignToNewField = new AssignToNewField(false, aliasName, expression, false, false);
      statements.Insert(0, assignToNewField);
   }

   public void RegisterReplacementTypeConstraint(ReplacementTypeConstraint replacementTypeConstraint)
   {
      replacementTypeConstraints[replacementTypeConstraint.Id] = replacementTypeConstraint;
   }

   public void ReplaceTypes(BaseClass originalClass, BaseClass newClass)
   {
      foreach (var replacementTypeConstraint in replacementTypeConstraints.Values.Where(r => r.Comparisands[0] == originalClass))
      {
         replacementTypeConstraint.Replace(newClass);
      }
   }
}