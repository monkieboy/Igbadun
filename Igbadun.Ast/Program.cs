using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Igbadun.Ast
{
    internal static class Program
    {
        private const string Indent = "    ";

        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: gen <output directory>");
                Environment.Exit(64);
            }

            var outputDir = args[0];
            DefineAst(outputDir, "Expr", new List<string>
            {
                "Assign   : Token Name, Expr Val",
                "Binary   : Expr Left, Token Op, Expr Right",
                "Grouping : Expr Expr",
                "Literal  : object Val",
                "Unary    : Token Op, Expr Right",
                "Mutable  : Token Name",
                "Value    : Token Name"
            });
            
            DefineAst(outputDir, "Stmt", new List<string>
            {
                "Block      : List<Stmt> Statements",
                "Expression : Expr Expr",
                "Print      : Expr Expr",
                "Mutable    : Token Name, Expr Initialiser",
                "Value      : Token Name, Expr Initialiser"
            });
        }

        private static void DefineAst(string outputDir, string name, List<string> types)
        {
            var path = Path.Combine(outputDir, $"{name}.cs");
            var fileWriter = new StringWriter();
            fileWriter.WriteLine("namespace Igbadun {");
            
            fileWriter.WriteLine($"{Indent}using System.Collections.Generic;");
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
            fileWriter.WriteLine($"{Indent}{Indent}public interface IVisitor<out T> {{");
            foreach (var type in types)
            {
                var typeName = type.Split(":")[0].Trim();
                fileWriter.WriteLine($"{Indent}{Indent}{Indent}T Visit{typeName+name}({typeName} {name.ToLower()});");
            }
            fileWriter.WriteLine($"{Indent}{Indent}}}");
        }

        private static void DefineType(StringWriter fileWriter, string name, string className, string fields)
        {
            var lowerCaseCtorArgsArray = fields.Split(' ').Select((x,n) => n%2!=0?x.ToLower() : x);
            var lowerCaseCtorArgs = String.Join(' ', lowerCaseCtorArgsArray);
            fileWriter.WriteLine($"{Indent}{Indent}public class {className} : {name} {{");
            fileWriter.WriteLine($"{Indent}{Indent}{Indent}public {className}({lowerCaseCtorArgs}) {{");
            foreach (var field in fields.Split(", "))
            {
                var fieldName = field.Split(" ")[1];
                fileWriter.WriteLine($"{Indent}{Indent}{Indent}{Indent}{fieldName} = {fieldName.ToLower()};");
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