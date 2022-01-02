using System;
using System.Collections.Generic;
using System.IO;

namespace Igbadun.Ast
{
    internal static class Program
    {
        private const string Indent = "    ";

        private static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "T")
            {
                var e = new Expr.Binary(
                    new Expr.Unary(
                        new Token(TokenType.MINUS, "-", null, 1),
                        new Expr.Literal(123)),
                    new Token(TokenType.STAR, "*", null, 1),
                    new Expr.Grouping(new Expr.Literal(45.67)));
                Console.WriteLine(new AstPrinter().Print(e));
                return;
            }
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: gen <output directory>");
                System.Environment.Exit(64);
            }

            var outputDir = args[0];
            DefineAst(outputDir, "Expr", new List<string>
            {
                "Assign   : Token name, Expr value",
                "Binary   : Expr left, Token op, Expr right",
                "Grouping : Expr expression",
                "Literal  : object value",
                "Unary    : Token op, Expr right",
                "Mutable  : Token name",
                "Value    : Token name"
            });
            
            DefineAst(outputDir, "Stmt", new List<string>
            {
                "Expression : Expr expression",
                "Print      : Expr expression",
                "Mutable    : Token name, Expr initialiser",
                "Value      : Token name, Expr initialiser"
            });
        }

        private static void DefineAst(string outputDir, string name, List<string> types)
        {
            var path = Path.Combine(outputDir, $"{name}.cs");
            var fileWriter = new StringWriter();
            fileWriter.WriteLine("namespace Igbadun {");
            
            fileWriter.WriteLine($"{Indent}using Igbadun;");
            fileWriter.WriteLine();
            
            fileWriter.WriteLine($"{Indent}public abstract class {name} {{");

            DefineVisitor(fileWriter, name, types);
            
            foreach (var type in types)
            {
                var className = type.Split(':')[0].Trim();
                var fields = type.Split(':')[1].Trim();
                DefineType(fileWriter, name, className, fields);
            }
            
            fileWriter.WriteLine();
            fileWriter.WriteLine($"{Indent}public abstract T Accept<T>(IVisitor<T> visitor);");
            fileWriter.WriteLine($"{Indent}}}");
            
            fileWriter.WriteLine("}");
            
            File.WriteAllText(path, fileWriter.ToString());
        }

        private static void DefineVisitor(StringWriter fileWriter, string name, List<string> types)
        {
            fileWriter.WriteLine($"{Indent}{Indent}public interface IVisitor<T> {{");
            foreach (var type in types)
            {
                var typeName = type.Split(":")[0].Trim();
                fileWriter.WriteLine($"{Indent}{Indent}{Indent}T Visit{typeName+name}({typeName} {name.ToLower()});");
            }
            fileWriter.WriteLine($"{Indent}{Indent}}}");
        }

        private static void DefineType(StringWriter fileWriter, string name, string className, string fields)
        {
            fileWriter.WriteLine($"{Indent}{Indent}public class {className} : {name} {{");
            fileWriter.WriteLine($"{Indent}{Indent}{Indent}public {className}({fields}) {{");
            foreach (var field in fields.Split(", "))
            {
                var fieldName = field.Split(" ")[1];
                fileWriter.WriteLine($"{Indent}{Indent}{Indent}{Indent}this.{fieldName} = {fieldName};");
            }
            fileWriter.WriteLine($"{Indent}{Indent}{Indent}}}");

            fileWriter.WriteLine();
            fileWriter.WriteLine($"{Indent}{Indent}{Indent}public override T Accept<T>(IVisitor<T> visitor) {{");
            fileWriter.WriteLine($"{Indent}{Indent}{Indent}{Indent}return visitor.Visit{className+name}(this);");
            fileWriter.WriteLine($"{Indent}{Indent}{Indent}}}");
            fileWriter.WriteLine();

            foreach (var field in fields.Split(", "))
            {
                fileWriter.WriteLine($"{Indent}{Indent}{Indent}public readonly {field};");
            }
            
            fileWriter.WriteLine($"{Indent}{Indent}}}");
        }
    }
}