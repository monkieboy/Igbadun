module Parsing.Program

open System
open Igbadun.Ast
open Igbadun.Lexing

let scan source =
    let scanner = Lexer(source)
    let tokens = scanner.ScanTokens()
    tokens

let runRepl() =
    let mutable running = true
    Console.WriteLine("Type 'exit' to exit the repl.")
    while running do
        Console.Write("> ")
        let cmd = Console.ReadLine()
        if (cmd = "exit" || cmd = "quit" || cmd = ":q") then running <- false
        else
            let tokens = scan cmd
            tokens |> Seq.iter(printfn "%A")
            if (not Lexer.Errored)
            then
                let p = Parser(tokens)
                AstPrinter().Print(p.parse()) |> printfn "%s"
            Lexer.Errored <- false
    Console.WriteLine("exiting...")

[<EntryPoint>]
let main args =
    runRepl()
    0