using System.Collections.Generic;

namespace Igbadun
{
    public class Environment
    {
        private readonly Dictionary<string, object> mutables = new();
        public Values Values { get; } = new();

        public void Define(string name, object value) => mutables.Add(name, value);

        public object Get(Token name)
        {
            if (mutables.ContainsKey(name.Lexeme))
            {
                return mutables[name.Lexeme];
            }
            if (Values.ContainsKey(name.Lexeme))
            {
                return Values.Get(name);
            }

            throw new RuntimeError(name, $"Undefined mutable '{name.Lexeme}'.");
        }

        public void Assign(Token name, object value)
        {
            if (mutables.ContainsKey(name.Lexeme))
            {
                mutables[name.Lexeme] = value;
                return;
            }

            throw new RuntimeError(name, $"Undefined mutable {name.Lexeme}. Perhaps you are trying to mutate a val?");
        }
    }

    public class Values
    {
        private readonly Dictionary<string, object> values = new();
        
        public object Get(Token name)
        {
            if (ContainsKey(name.Lexeme))
            {
                return values[name.Lexeme];
            }

            throw new RuntimeError(name, $"Undefined val '{name.Lexeme}'.");
        }

        public void Define(string name, object value) => values.Add(name, value);

        public bool ContainsKey(string lexeme)
        {
            return values.ContainsKey(lexeme);
        }
    }
}