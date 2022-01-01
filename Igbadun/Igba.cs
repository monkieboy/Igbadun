using System;
using System.Collections.Generic;
using System.IO;
using static Igbadun.Scanner;

namespace Igbadun
{
    internal static class Igba
    {
        private static readonly Interpreter Interpreter = new();
        private static bool showParse = false;

        private static void Main(string[] args)
        {
            switch (args.Length)
            {
                case > 2:
                    Console.WriteLine("To use this program: igba [script]");
                    System.Environment.Exit(64);
                    break;
             
                case 1:
                    RunFile(args[0]);
                    break;

                default:
                    //showParse = true;
                    RunRepl();
                    break;
            }
        }

        private static void Run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();
            var parser = new Parser(tokens);
            var statements = parser.Parse();
            if(Errored) System.Environment.Exit(65);

            Interpreter.Interpret(statements);
            if(RuntimeErrored) System.Environment.Exit(70);

            if (showParse)
            {
                Console.WriteLine(new AstPrinter().Print(statements));
            }
        }

        private static void RunFile(string filePath)
        {
            var source = File.ReadAllText(filePath);
            Run(source);
        }


        private static void RunRepl()
        {   
            Console.WriteLine("Type 'exit' to exit the repl.");
            while (true)
            {
                Console.Write("> ");
                var cmd = Console.ReadLine();
                if (cmd is "exit" or "quit" or ":q" or ":Q") break;
                Run(cmd);
                Errored = false;
                RuntimeErrored = false;
            }

            Console.WriteLine("exiting...");
        }
    }
}