using System;
using System.Collections.Generic;
using System.Globalization;

namespace Igbadun
{
    public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
    {
        private readonly Environment environment = new ();

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }
        public void Interpret(List<Stmt> statements)
        {
            try
            {
                // var value = Evaluate(expr);
                // Console.WriteLine(Stringify(value));
                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (RuntimeError e)
            {
                Scanner.RuntimeError(e);
            }
        }

        private string Stringify(object o)
        {
            if (o == null) return "nil";
            if (o is double d)
            {
                var text = d.ToString(CultureInfo.InvariantCulture);
                if (text.EndsWith(".0"))
                {
                    text = text.Substr(0, text.Length - 2);
                }

                return text;
            }

            return o.ToString();
        }

        public object VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.expression);
            return null;
        }

        public object VisitPrintStmt(Stmt.Print stmt)
        {
            var text = Evaluate(stmt.expression);
            Console.WriteLine(Stringify(text));
            return null;
        }

        public object VisitMutableStmt(Stmt.Mutable stmt)
        {
            object value = null;
            if (stmt.initialiser != null)
            {
                value = Evaluate(stmt.initialiser);
            }

            environment.Define(stmt.name.Lexeme, value);
            return null;
        }

        public object VisitMutableExpr(Expr.Mutable expr)
        {
            return environment.Get(expr.name);
        }
        
        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.value;
        }

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.expression);
        }

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            var right = Evaluate(expr.right);
            switch (expr.op.TokenType)
            {
                case TokenType.MINUS:
                    CheckNumericOp(expr.op, right);
                    return -(double) right;
                case TokenType.BANG:
                    return !IsTruthy(right);
                default:
                    return null;
            }
        }

        private void CheckNumericOp(Token op, object operand)
        {
            if (operand is double) return;
            throw new RuntimeError(op, $"The operand argument is not a Double; operand '{operand}' is type {operand.GetType()}");
        }

        private void CheckNumericOp(Token op, object leftOperand, object rightOperand)
        {
            if (leftOperand is double && rightOperand is double) return;
            throw new RuntimeError(op, $"The operand arguments are not Doubles; left operand '{leftOperand}' is type {leftOperand.GetType()} and right operand '{rightOperand}' is type {rightOperand.GetType()}");
        }

        public object VisitAssignExpr(Expr.Assign expr)
        {
            var value = Evaluate(expr.value);
            environment.Assign(expr.name, value);
            return value;
        }

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            var left = Evaluate(expr.left);
            var right = Evaluate(expr.right);

            switch (expr.op.TokenType)
            {
                case TokenType.PLUS:
                    return left switch
                    {
                        string s1 when right is string s2 => s1 + s2,
                        double d1 when right is double d2 => d1 + d2,
                        _ => throw new RuntimeError(expr.op, $"Operands must two numbers or two strings, numeric or string. The left operand '{left}' is type of {left.GetType()} and the right operand '{right}' is type of {right.GetType()}.")
                    };
                case TokenType.MINUS:
                    CheckNumericOp(expr.op,left,right);
                    return (double) left - (double) right;      
                case TokenType.STAR:
                    CheckNumericOp(expr.op,left,right);
                    return (double) left * (double) right;      
                case TokenType.FORWARD_SLASH:
                    CheckNumericOp(expr.op,left,right);
                    return (double) left / (double) right;      
                case TokenType.PERCENT:
                    CheckNumericOp(expr.op,left,right);
                    return (double) left % (double) right;      
                case TokenType.POWER:
                    CheckNumericOp(expr.op,left,right);
                    return Math.Pow((double) left, (double) right);
                case TokenType.EQUAL:
                    return IsEqual(left, right);
                case TokenType.GREATER:
                    CheckNumericOp(expr.op,left,right);
                    return (double) left > (double) right;      
                case TokenType.LESS:
                    CheckNumericOp(expr.op,left,right);
                    return (double) left < (double) right;
                case TokenType.NOT_EQUAL:
                    return !IsEqual(left, right);      
                case TokenType.GREATER_OR_EQUAL:
                    CheckNumericOp(expr.op,left,right);
                    return (double) left >= (double) right;      
                case TokenType.LESS_OR_EQUAL:
                    CheckNumericOp(expr.op,left,right);
                    return (double) left <= (double) right;      
                default:
                    return null;
            }
        }

        private static bool IsEqual(object left, object right)
        {
            return left switch
            {
                null when right is null => true,
                null => false,
                _ => left.Equals(right)
            };
        }

        private static bool IsTruthy(object value) =>
            value switch
            {
                null => false,
                bool b => b,
                _ => true
            };

        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }
        
    }
}