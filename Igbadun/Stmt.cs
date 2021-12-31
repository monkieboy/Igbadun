namespace Igbadun {
    using Igbadun;

    public abstract class Stmt {
        public interface IVisitor<T> {
            T VisitExpressionStmt(Expression stmt);
            T VisitPrintStmt(Print stmt);
            T VisitMutableStmt(Mutable stmt);
        }
        public class Expression : Stmt {
            public Expression(Expr expression) {
                this.expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitExpressionStmt(this);
            }

            public readonly Expr expression;
        }
        public class Print : Stmt {
            public Print(Expr expression) {
                this.expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitPrintStmt(this);
            }

            public readonly Expr expression;
        }
        public class Mutable : Stmt {
            public Mutable(Token name, Expr initialiser) {
                this.name = name;
                this.initialiser = initialiser;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitMutableStmt(this);
            }

            public readonly Token name;
            public readonly Expr initialiser;
        }

    public abstract T Accept<T>(IVisitor<T> visitor);
    }
}
