namespace Igbadun {
    public abstract class Stmt {
        public interface IVisitor<out T> {
            T VisitExpressionStmt(Expression stmt);
            T VisitPrintStmt(Print stmt);
            T VisitMutableStmt(Mutable stmt);
            T VisitValueStmt(Value stmt);
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
                this.Name = name;
                this.Initialiser = initialiser;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitMutableStmt(this);
            }

            public readonly Token Name;
            public readonly Expr Initialiser;
        }
        public class Value : Stmt {
            public Value(Token name, Expr initialiser) {
                this.Name = name;
                this.Initialiser = initialiser;
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
