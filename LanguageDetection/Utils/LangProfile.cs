using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace LanguageDetection.Utils
{
    /// <summary>
    /// {@link LangProfile} is a Language Profile Class.
    /// Users don't use this class directly.
    /// </summary>
    public class LangProfile
    {
        private const int MINIMUM_FREQ = 2;
        private const int LESS_FREQ_RATIO = 100000;

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("freq")]
        public Dictionary<string, int> Freq { get; set; }

        [JsonPropertyName("n_words")]
        public int[] N_Words { get; set; }

        /// <summary>
        /// Constructor for JSONIC
        /// </summary>
        public LangProfile()
        {
        }

        /// <summary>
        /// Normal Constructor
        /// </summary>
        /// <param name="name">language name</param>
        public LangProfile(string name)
        {
            Name = name;
            Freq = new Dictionary<string, int>();
            N_Words = new int[NGram.N_GRAM];
        }

        /// <summary>
        /// Add n-gram to profile
        /// </summary>
        /// <param name="gram"></param>
        public void Add(string gram)
        {
            if (Name == null || gram == null) return;   // Illegal
            int len = gram.Length;
            if (len < 1 || len > NGram.N_GRAM) return;  // Illegal
            ++N_Words[len - 1];
            if (Freq.ContainsKey(gram))
            {
                Freq[gram] = Freq[gram] + 1;
            }
            else
            {
                Freq[gram] = 1;
            }
        }

        /// <summary>
        /// Eliminate below less frequency n-grams and noise Latin alphabets
        /// </summary>
        public void OmitLessFreq()
        {
            if (Name == null) return;   // Illegal
            int threshold = N_Words[0] / LESS_FREQ_RATIO;
            if (threshold < MINIMUM_FREQ) threshold = MINIMUM_FREQ;

            string[] keys = Freq.Keys.ToArray();
            int roman = 0;
            foreach (string key in keys)
            {
                int count = Freq[key];
                if (count <= threshold)
                {
                    N_Words[key.Length - 1] -= count;
                    Freq.Remove(key);
                }
                else
                {
                    if (Regex.IsMatch(key, "^[A-Za-z]$"))
                    {
                        roman += count;
                    }
                }
            }

            // roman check
            if (roman < N_Words[0] / 3)
            {
                string[] keys2 = Freq.Keys.ToArray();
                foreach (string key in keys2)
                {
                    if (Regex.IsMatch(key, ".*[A-Za-z].*"))
                    {
                        N_Words[key.Length - 1] -= Freq[key];
                        Freq.Remove(key);
                    }
                }
            }
        }

        /// <summary>
        /// Update the language profile with (fragmented) text.
        /// Extract n-grams from text and add their frequency into the profile.
        /// </summary>
        /// <param name="text">(fragmented) text to extract n-grams</param>

        public void Update(string text)
        {
            if (text == null) return;
            text = NGram.normalize_vi(text);
            NGram gram = new NGram();
            for (int i = 0; i < text.Length; ++i)
            {
                gram.AddChar(text[i]);
                for (int n = 1; n <= NGram.N_GRAM; ++n)
                {
                    Add(gram[n]);
                }
            }
        }
    }
}

