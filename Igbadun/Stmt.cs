namespace Igbadun {
    using System.Collections.Generic;

    public abstract class Stmt {
        public interface IVisitor<out T> {
            T VisitBlockStmt(Block stmt);
            T VisitExpressionStmt(Expression stmt);
            T VisitPrintStmt(Print stmt);
            T VisitMutableStmt(Mutable stmt);
            T VisitValueStmt(Value stmt);
        }
        public class Block : Stmt {
            public Block(List<Stmt> statements) {
                Statements = statements;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitBlockStmt(this);
            }

            public readonly List<Stmt> Statements;
        }
        public class Expression : Stmt {
            public Expression(Expr expr) {
                Expr = expr;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitExpressionStmt(this);
            }

            public readonly Expr Expr;
        }
        public class Print : Stmt {
            public Print(Expr expr) {
                Expr = expr;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitPrintStmt(this);
            }

            public readonly Expr Expr;
        }
        public class Mutable : Stmt {
            public Mutable(Token name, Expr initialiser) {
                Name = name;
                Initialiser = initialiser;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitMutableStmt(this);
            }

            public readonly Token Name;
            public readonly Expr Initialiser;
        }
        public class Value : Stmt {
            public Value(Token name, Expr initialiser) {
                Name = name;
                Initialiser = initialiser;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitValueStmt(this);
            }

            public readonly Token Name;
            public readonly Expr Initialiser;
        }

    public abstract T Accept<T>(IVisitor<T> visitor);
    }
}
