using System;
using System.Collections.Generic;
using static Igbadun.TokenType;

namespace Igbadun
{
    public class Scanner
    {
        public static bool Errored;
        public static bool RuntimeErrored;
        private readonly string source; 
        private readonly List<Token> tokens = new();
        private int start;
        private int current;
        private int line = 1;

        private static readonly Dictionary<string, TokenType> ReservedWords = new()
        {
            {"nil", NIL},
            {"and", AND},
            {"or", OR},
            {"true", TRUE},
            {"false", FALSE},
            {"print", PRINT},
            {"return", RETURN},
            {"val", VAL},
            {"mutable", MUTABLE},
            {"if", IF},
            {"else", ELSE},
            {"fun", FUN},
        };

        public Scanner(string source)
        {
            this.source = source;
        }

        private bool IsEndOfSource() => current >= source.Length;

        public List<Token> ScanTokens()
        {
            while (!IsEndOfSource())
            {
                start = current;
                ScanToken();
            }
            tokens.Add(new Token(EOF, string.Empty, null, line));
            return tokens;
        }

        char NextChar() => source[current++];
        void AddToken(TokenType tokenType) => AddToken(tokenType, null);

        void AddToken(TokenType tokenType, object literal)
        {
            var text = source.Substr(start, current);
            tokens.Add(new Token(tokenType, text, literal, line));
        }

        private void ScanToken()
        {
            var currentChar = NextChar();
            switch (currentChar)
            {
                case ' ': case '\r': case '\t': break;
                case '\n': line++; break;
                case '(': AddToken(LEFT_PAREN); break;
                case ')': AddToken(RIGHT_PAREN); break;
                case '{': AddToken(LEFT_BRACE); break;
                case '}': AddToken(RIGHT_BRACE); break;
                case '[': AddToken(LEFT_SQUARE); break;
                case ']': AddToken(RIGHT_SQUARE); break;
                case ',': AddToken(COMMA); break;
                case '.': AddToken(DOT); break;
                case '+': AddToken(PLUS); break;
                case '-': AddToken(MINUS); break;
                case '*': AddToken(STAR); break;
                case '/': AddToken(FORWARD_SLASH); break;
                case '\\': AddToken(BACK_SLASH); break;
                case ';': AddToken(SEMI_COLON); break;
                case ':': 
                    AddToken(COLON); break;
                case '#':
                    while (Peek() != '\n' && !IsEndOfSource())
                    {
                        NextChar();
                    }
                    AddToken(HASH); break;
                case '!':
                    AddToken(
                        Match('=')
                            ? NOT_EQUAL
                            : BANG
                    );
                    
                    break;
                case '=': AddToken(EQUAL); break;
                case '<':
                    var lessOrEqual = Match('=')
                        ? LESS_OR_EQUAL
                        : LESS;
                    var mutateOrEval = Match('-') ? MUTABLE : lessOrEqual;
                    AddToken(mutateOrEval);
                    break;
                case '>': 
                    AddToken(
                        Match('=')
                            ? GREATER_OR_EQUAL
                            : GREATER); 
                    break;
                case '"':
                    StringLiteral();
                    break;
                case '\'': 
                    break;
                case '%': AddToken(PERCENT); break;
                case '^': AddToken(POWER); break;
                default:
                    if (IsAlpha(currentChar))
                    {
                        Identifier();
                    }
                    else if (IsNumeric(currentChar))
                        Numeric();
                    else
                        Error(line, $"Unexpected character: {currentChar}");
                    break;
            }
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) NextChar();
            var text = source.Substr(start, current);
            var tokenType = ReservedWords.ContainsKey(text) 
                ? ReservedWords[text]
                : IDENTIFIER;
            AddToken( tokenType );
        }
        private void Numeric()
        {
            while (IsNumeric(Peek())) NextChar();
            if (Peek() == '.' && IsNumeric(PeekNext()))
            {
                NextChar();
                while (IsNumeric(Peek())) NextChar();
            }
            
            AddToken(NUMERIC, Double.Parse(source.Substr(start, current)));
        }

        private void StringLiteral()
        {
            while (Peek() != '"' && !IsEndOfSource())
            {
                if (Peek() == '\n') line++;
                NextChar();
            }   

            if (IsEndOfSource())
            {
                Error(line, "The string is missing closing double quote.");
                return;
            }

            NextChar();
            var text = source.Substr(start + 1, current - 1);
            AddToken(STRING, text);
        }

        private static bool IsAlpha(char c) => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';

        private static bool IsAlphaNumeric(char c) => IsAlpha(c) || IsNumeric(c);

        private static bool IsNumeric(char c) => c is >= '0' and <= '9';
        
        private char Peek() => IsEndOfSource() ? '\0' : source[current];

        private char PeekNext() => current + 1 >= source.Length ? '\0' : source[current + 1];

        private bool Match(char pattern)
        {
            if (IsEndOfSource()) return false;
            if (source[current] != pattern) return false;
            current++;
            return true;
        }


        // Move the `Error` and `Report` functions to another module for error reporting
        private static void Error(int line, string msg) => Report(line, string.Empty, msg);

        private static void Report(int line, string position, string msg)
        {
            Console.Error.WriteLine($"[line {line}] Error {position}: {msg}");
            Errored = true;
        }

        public static void Error(Token token, string msg)
        {
            var position =
                token.TokenType == EOF 
                    ? " at end" 
                    : $" at '{token.Lexeme}'";
            Report(token.Line, position, msg);
        }

        public static void RuntimeError(RuntimeError e)
        {
            Console.Error.WriteLine($"{e.Message}\n[line {e.Token.Line}]");
            RuntimeErrored = true;
        }
    }
}