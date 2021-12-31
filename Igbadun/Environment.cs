using System.Collections.Generic;

namespace Igbadun
{
    public class Environment
    {
        private Dictionary<string, object> mutables = new();

        public void Define(string name, object value) => mutables.Add(name, value);

        public object Get(Token name)
        {
            if (mutables.ContainsKey(name.Lexeme))
            {
                return mutables[name.Lexeme];
            }

            throw new RuntimeError(name, $"Undefined mutable '{name.Lexeme}'.");
        }
    }
}