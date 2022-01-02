namespace Igbadun {
    public abstract class Expr {
        public interface IVisitor<out T> {
            T VisitAssignExpr(Assign expr);
            T VisitBinaryExpr(Binary expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitUnaryExpr(Unary expr);
            T VisitMutableExpr(Mutable expr);
            T VisitValueExpr(Value expr);
        }
        public class Assign : Expr {
            public Assign(Token name, Expr value) {
                Name = name;
                Val = value;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitAssignExpr(this);
            }

            public readonly Token Name;
            public readonly Expr Val;
        }
        public class Binary : Expr {
            public Binary(Expr left, Token op, Expr right) {
                Left = left;
                Op = op;
                Right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitBinaryExpr(this);
            }

            public readonly Expr Left;
            public readonly Token Op;
            public readonly Expr Right;
        }
        public class Grouping : Expr {
            public Grouping(Expr expression) {
                Expr = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitGroupingExpr(this);
            }

            public readonly Expr Expr;
        }
        public class Literal : Expr {
            public Literal(object value) {
                Val = value;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitLiteralExpr(this);
            }

            public readonly object Val;
        }
        public class Unary : Expr {
            public Unary(Token op, Expr right) {
                Op = op;
                Right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitUnaryExpr(this);
            }

            public readonly Token Op;
            public readonly Expr Right;
        }
        public class Mutable : Expr {
            public Mutable(Token name) {
                Name = name;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitMutableExpr(this);
            }

            public readonly Token Name;
        }
        public abstract class Value : Expr {
            protected Value(Token name) {
                Name = name;
            }

            public override T Accept<T>(IVisitor<T> visitor) {
                return visitor.VisitValueExpr(this);
            }

            public readonly Token Name;
        }

    public abstract T Accept<T>(IVisitor<T> visitor);
    }
}
