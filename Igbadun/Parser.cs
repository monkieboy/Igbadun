using System.Collections.Generic;
using System.Linq;
using static Igbadun.TokenType;

namespace Igbadun
{
    public class Parser
    {
        private readonly List<Token> tokens;
        private int current = 0;
        
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        Token Previous() => tokens[current - 1];
        Token Peek() => tokens[current];
        bool IsEndOfSource() => Peek().TokenType == EOF;

        Token Advance()
        {
            if (!IsEndOfSource()) current++;
            return Previous();
        }

        bool Check(TokenType tokenType)
        {
            var token = Peek();
            return !IsEndOfSource() && token.TokenType == tokenType;
        }

        private bool Match(params TokenType[] types)
        {
            if (!types.Any(t=> Check(t))) return false;
            Advance();
            return true;
        }

        ParseError Error(Token token, string message)
        {
            Scanner.Error(token, message);
            return new ParseError(message);
        }

        void Synchronise()
        {
            Advance();
            while (!IsEndOfSource())
            {
                if (Previous().TokenType == SEMI_COLON)
                {
                    return;
                }

                switch (Peek().TokenType)
                {
                    case FUN : case IF : case PRINT : case RETURN : case MUTABLE : case VAL:
                        return;
                }

                Advance();
            }
        }

        Token Consume(TokenType tokenType, string message)
        {
            if (Check(tokenType)) return Advance();
            
            throw Error(Peek(), message);
        }

        Expr Primary()
        {
            if (Match(FALSE)) return new Expr.Literal(false);
            if (Match(TRUE)) return new Expr.Literal(true);
            if (Match(NIL)) return new Expr.Literal(null);
            if (Match(NUMERIC, STRING)) return new Expr.Literal(Previous().Literal);
            if (Match(IDENTIFIER)) return new Expr.Mutable(Previous());
            if (Match(LEFT_PAREN))
            {
                var expr = Expression();
                Consume(RIGHT_PAREN, "Expect ')'after expression.");
                return new Expr.Grouping(expr);
            }

            throw Error(Peek(), "Expected an expression");
        }

        Expr Unary()
        {
            if (Match(BANG, MINUS))
            {
                var op = Previous();
                var right = Unary();
                return new Expr.Unary(op, right);
            }

            return Primary();
        }

        Expr Factor()
        {
            var expr = Unary();
            while (Match(FORWARD_SLASH, STAR, PERCENT, POWER))
            {
                var op = Previous();
                var right = Unary();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        Expr Term()
        {
            var expr = Factor();
            while (Match(MINUS, PLUS))
            {
                var op = Previous();
                var right = Factor();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        
        Expr Comparison()
        {
            var expr = Term();
            while (Match(GREATER, GREATER_OR_EQUAL, LESS, LESS_OR_EQUAL))
            {
                var op = Previous();
                var right = Term();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        Expr Equality()
        {
            var expr = Comparison();
            while (Match(NOT_EQUAL, EQUAL))
            {
                var op = Previous();
                var right = Comparison();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Assignment()
        {
            var expr = Equality();
            if (Match(EQUAL, MUTABLE)) // here to apply fix
            {
                var equals = Previous();
                var value = Assignment();

                if (expr is Expr.Mutable)
                {
                    var name = ((Expr.Mutable) expr).name;
                    return new Expr.Assign(name, value);
                }

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }
        private Expr Expression() => Assignment();

        private Stmt PrintStatement()
        {
            var text = Expression();
            ExpectSemiColon();
            return new Stmt.Print(text);
        }

        private Stmt ExpressionStatement()
        {
            var expr = Expression();
            ExpectSemiColon();
            return new Stmt.Expression(expr);
        }

        private void ExpectSemiColon()
        {
            Consume(SEMI_COLON, "Expected a ';' after value.");
        }

        private Stmt Statement()
        {
            if (Match(PRINT)) return PrintStatement();
            return ExpressionStatement();
        }

        private Stmt MutableDeclaration()
        {
            var name = Consume(IDENTIFIER, "Expected a variable name.");
            Expr initialiser = null;
            if (Match(EQUAL))
            {
                initialiser = Expression();
            }

            Consume(SEMI_COLON, "Expected a ';' after the variable declaration.");
            return new Stmt.Mutable(name, initialiser);
        }
        private Stmt Declaration()
        {
            try
            {
                if (Match(MUTABLE)) return MutableDeclaration();
                return Statement();
            }
            catch (ParseError)
            {
                Synchronise();
                return null;
            }
        }
        public List<Stmt> Parse()
        {
            var statements = new List<Stmt>();
            while (!IsEndOfSource())
            {
                statements.Add(Declaration());
            }

            return statements;
        }
        
        // public Expr Parse()
        // {
        //     try
        //     {
        //         return Expression();
        //     }
        //     catch (ParseError)
        //     {
        //         return null;
        //     }
        // }
    }
}