namespace Igbadun {
    using Igbadun;

    public abstract class Expr {
        public interface IVisitor<T> {
            T VisitAssignExpr(Assign expr);
            T VisitBinaryExpr(Binary expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitUnaryExpr(Unary expr);
            T VisitMutableExpr(Mutable expr);
        }
        public class Assign : Expr {
            public Assign(Token name, Expr value) {
                this.name = name;
                this.value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitAssignExpr(this);
            }

            public readonly Token name;
            public readonly Expr value;
        }
        public class Binary : Expr {
            public Binary(Expr left, Token op, Expr right) {
                this.left = left;
                this.op = op;
                this.right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitBinaryExpr(this);
            }

            public readonly Expr left;
            public readonly Token op;
            public readonly Expr right;
        }
        public class Grouping : Expr {
            public Grouping(Expr expression) {
                this.expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitGroupingExpr(this);
            }

            public readonly Expr expression;
        }
        public class Literal : Expr {
            public Literal(object value) {
                this.value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitLiteralExpr(this);
            }

            public readonly object value;
        }
        public class Unary : Expr {
            public Unary(Token op, Expr right) {
                this.op = op;
                this.right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitUnaryExpr(this);
            }

            public readonly Token op;
            public readonly Expr right;
        }
        public class Mutable : Expr {
            public Mutable(Token name) {
                this.name = name;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitMutableExpr(this);
            }

            public readonly Token name;
        }

    public abstract T Accept<T>(IVisitor<T> visitor);
    }
}
