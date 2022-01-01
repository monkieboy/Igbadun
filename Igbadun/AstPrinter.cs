using System.Collections.Generic;
using System.Text;

namespace Igbadun
{
    public class AstPrinter : Expr.IVisitor<string>, Stmt.IVisitor<object>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string Print(List<Stmt> statement)
        {
            return "";
//            return statement.Accept(this);
        }

        public object VisitExpressionStmt(Stmt.Expression stmt)
        {
            throw new System.NotImplementedException();
        }

        public object VisitPrintStmt(Stmt.Print stmt)
        {
            throw new System.NotImplementedException();
        }

        public object VisitMutableStmt(Stmt.Mutable stmt)
        {
            throw new System.NotImplementedException();
        }

        public string VisitAssignExpr(Expr.Assign expr)
        {
            throw new System.NotImplementedException();
        }

        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return Parenthesize(expr.op.Lexeme, expr.left, expr.right);
        }

        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            return Parenthesize("group", expr.expression);
        }

        public string VisitLiteralExpr(Expr.Literal expr)
        {
            if (expr.value == null) return "nil";
            return expr.value.ToString();
        }

        public string VisitUnaryExpr(Expr.Unary expr)
        {
            return Parenthesize(expr.op.Lexeme, expr.right);
        }

        public string VisitMutableExpr(Expr.Mutable expr)
        {
            throw new System.NotImplementedException();
        }

        private string Parenthesize(string name, params Expr[] exprs)
        {
            var builder = new StringBuilder();
            builder.Append('(').Append(name);
            foreach (var expr in exprs)
            {
                builder.Append(' ');
                builder.Append(expr.Accept(this));
            }

            builder.Append(')');
            return builder.ToString();
        }
    }
}