namespace Parsing

open System
open Igbadun

open Igbadun.Lexing

exception ParseError of string

type Parser(tokens:ResizeArray<Token>) =
    let tokens = tokens
    let mutable current = 0
    
    member this.previous() = tokens.[current-1]
    member this.peek() = tokens.[current]
    member this.eol = this.peek().TokenType = TokenType.EOF
    member this.advance() =
        if not this.eol then current <- current+1
        this.previous()
    member this.check (tokenType:TokenType) =
        not this.eol && this.peek().TokenType = tokenType
    
    member this.combine (types:TokenType list) =
        match types |> Seq.tryFind this.check with
        | Some _ -> this.advance() |> ignore; true
        | None -> false
        
    member this.error(token, message:string) =
        Lexer.Error(token, message)
        ParseError message
        
    member this.synchronise() =
        this.advance() |> ignore
        while (not <| this.eol) do
            if (this.previous().TokenType = TokenType.SEMI_COLON)
            then ()
            else
                match this.peek().TokenType with
                | TokenType.FUN | TokenType.IF | TokenType.PRINT | TokenType.RETURN | TokenType.MUTABLE | TokenType.VAL -> ()
                | _ -> this.advance() |> ignore
        
    member this.consume(tokenType, message) =
        if (this.check(tokenType)) then this.advance()
        else raise (this.error(this.peek(), message))
        
    member this.primary() : Expr =
        if this.combine [TokenType.FALSE] then
            Expr.Literal(false) :> Expr
        elif this.combine [TokenType.TRUE] then
            Expr.Literal(true) :> Expr
        elif this.combine [TokenType.NUMERIC; TokenType.STRING] then
            Expr.Literal(this.previous().Literal) :> Expr
        elif this.combine [TokenType.LEFT_PAREN] then
            let expr = this.expression()
            this.consume(TokenType.RIGHT_PAREN, "Expect ')' after expression") |> ignore
            Expr.Grouping(expr) :> Expr
        else
            raise (this.error(this.peek(), "Expect expression"))
    member this.unary() =
        match this.combine [TokenType.BANG; TokenType.MINUS] with
        | true ->
            let operator = this.previous()
            let right = this.unary()
            Expr.Unary(operator, right) :> Expr
        | _ -> this.primary()
        
    member this.factor() =
        let mutable expr = this.unary()
        while (this.combine [TokenType.FORWARD_SLASH; TokenType.STAR]) do
            let operator = this.previous()
            let right = this.unary()
            expr <- Expr.Binary(expr, operator, right)
        expr
        
    member this.term() =
        let mutable expr = this.factor()
        
        while (this.combine [TokenType.MINUS; TokenType.PLUS]) do
            let operator = this.previous()
            let right = this.factor()
            expr <- Expr.Binary(expr,operator,right)
        expr
        
    member this.comparison() =
        let mutable expr = this.term()
        
        while (this.combine [TokenType.GREATER; TokenType.GREATER_OR_EQUAL; TokenType.LESS; TokenType.LESS_OR_EQUAL]) do
            let operator = this.previous()
            let right = this.term()
            expr <- Expr.Binary(expr, operator, right)
        expr

    member this.equality() =
        let mutable expr = this.comparison()
        while (this.combine [TokenType.NOT_EQUAL; TokenType.EQUAL]) do
            let operator = this.previous()
            let right = this.comparison()
            expr <- Expr.Binary(expr, operator, right)
        expr
        
    member this.expression() = this.equality()
    
    member this.parse() =
        try
            this.expression()
        with e ->
            printf $"{e.Message}"
            raise e
    
    
    
    
    
    
    
    
    
    
    
    
    
//    let rec parse(expr:Expr) =
//        match expr with
//        | Literal (Numeric n) -> n
//        | Literal (String _ as s) -> s
//        | Literal (Bool _ as b) -> b
//        | Unary(Not b) -> not b
//        | Unary(Negate n) -> -n
//        | Binary(e1, Equals, e2) -> parse e1 = parse e2
//        | Binary(e1, Greater, e2) -> parse e1 > parse e2
//        | Binary(e1, GreaterThanOrEqual, e2) -> parse e1 >= parse e2
//        | Binary(e1, Less, e2) -> parse e1 < parse e2
//        | Binary(e1, LessThanOrEqual, e2) -> parse e1 <= parse e2
//        | Binary(e1, Plus, e2) -> (parse e1) + (parse e2)
//        | Binary(e1, Minus, e2) -> (parse e1) - (parse e2)
//        | Binary(e1, Multiply, e2) -> (parse e1) * (parse e2)
//        | Binary(e1, Divide, e2) -> (parse e1) / (parse e2)
//        | _ -> failwith "todo"
//    