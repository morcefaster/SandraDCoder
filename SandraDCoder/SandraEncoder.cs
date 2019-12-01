using System;
using System.Collections.Generic;
using System.Text;

namespace SandraDCoder
{
    public class SandraEncoder
    {
        public string Encode(string text, string key)
        {
            text = text.ToUpper();
            var fullKey = key;
            while (fullKey.Length < Constants.Alphabet.Length)
            {
                fullKey += key;
            }

            text = text.ToUpper();

            var output = new StringBuilder();
            foreach (var letter in text)
            {
                if (Constants.Alphabet.Contains(letter))
                {
                    output.Append(fullKey[Constants.Alphabet.IndexOf(letter)]);
                }
                else
                {
                    output.Append(letter);
                }
            }

            return output.ToString();
        }
    }
}