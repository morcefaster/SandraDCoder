using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SandraDCoder
{
    public class SandraDecoder
    {
        private HashSet<string> Dictionary;

        public List<List<string>> Decode(string text, string key, bool heavyDictionary = false)
        {
            text = text.ToUpper();
            key = key.ToUpper();


            while (key.Length < Constants.Alphabet.Length)
            {
                key += key;
            }

            InitDict(heavyDictionary);

            var words = text.Split(' ');
            var resultWords = new List<List<string>>();
            foreach (var word in words)
            {
                resultWords.Add(DecodeWord(word, key));
            }

            var maxNum = resultWords.Max(rw => rw.Count);
            for (int k = 0; k < resultWords.Count; k++)
            {
                if (resultWords[k].Count < maxNum)
                {
                    var cnt = maxNum - resultWords[k].Count;
                    for (int i = 0; i < cnt; i++)
                    {
                        resultWords[k].Add(new String(' ', words[k].Length));
                    }
                }
            }

            for(int i = 0; i < resultWords.Count; i++)
            {
                resultWords[i].Insert(0,words[i]);
            }

            return resultWords;
        }

        public List<KeyValuePair<string, List<List<string>>>> DecodeWithoutKey(string text, bool heavyDictionary)
        {
            var result = new List<KeyValuePair<string, List<List<string>>>>();
            text = text.ToUpper();
            InitDict(true);
            var uniqueLetterList = new StringBuilder();
            foreach (var letter in text)
            {
                if (char.IsLetter(letter) && !uniqueLetterList.ToString().Contains(letter))
                {
                    uniqueLetterList.Append(letter);
                }
            }

            var possibleKeys = PossibleWords(uniqueLetterList.ToString()).ToList();
            InitDict(heavyDictionary);
            foreach (var key in possibleKeys)
            {
                result.Add(new KeyValuePair<string, List<List<string>>>(key, Decode(text, key, heavyDictionary)));
            }

            var ret = result.OrderBy(kvp => GetMinWords(kvp.Value))
                .Where(kvp => GetMinWords(kvp.Value) != kvp.Value.Count).ToList();
            if (ret.Count == 0)
            {
                return new List<KeyValuePair<string, List<List<string>>>>()
                {
                    new KeyValuePair<string, List<List<string>>>("NO MATCHING KEY FOUND", new List<List<string>>(){new List<string>(){"N/A"}})
                };
            }
            else
            {
                return ret;
            }
        }

        private int GetMinWords(List<List<string>> words)
        {
            var ret=   words.Count(l => l.Count(w => !string.IsNullOrWhiteSpace(w)) == 1);
            return ret;
        }

        private IEnumerable<string> PossibleWords(string uniqueChars)
        {
            foreach (var word in Dictionary)
            {
                if (uniqueChars.All(word.Contains))
                {
                    yield return word;
                }
            }
        }

        private void InitDict(bool heavy)
        {
            Dictionary = (heavy ? Dict.HeavyDictionary : Dict.CommonDictonary).Split('\n')
                .Select(t => t.Trim().ToUpper()).ToHashSet();
        }

        public List<string> DecodeWord(string word, string key, string currentWord = "")
        {
            if (currentWord.Length == word.Length)
            {
                if (Dictionary.Contains(currentWord))
                    return new List<string>() { currentWord };
                else
                {
                    return new List<string>();
                }
            }

            var totalList = new List<string>();
            if (Dictionary.Any(w => w.StartsWith(currentWord)))
            {
                foreach (var letter in KeyPositionList(word[currentWord.Length], key))
                {
                    totalList.AddRange(DecodeWord(word, key, currentWord + letter));
                }
            }

            return totalList;
        }

        private IEnumerable<char> KeyPositionList(char letter, string key)
        {
            for (int i = 0; i < Constants.Alphabet.Length; i++)
            {
                if (key[i] == letter)
                {
                    yield return Constants.Alphabet[i];
                }
            }
        }
    }
}
