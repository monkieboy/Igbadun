using System;

namespace Igbadun
{
    public class ParseError : Exception
    {
        public ParseError(string message) : base(message)
        {
        }
    }
}