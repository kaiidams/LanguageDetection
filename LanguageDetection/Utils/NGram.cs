using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Unicode;

namespace LanguageDetection.Utils
{
    /// <summary>
    /// Cut out N-gram from text. 
    /// Users don't use this class directly.
    /// </summary>
    public class NGram
    {
        private static readonly string LATIN1_EXCLUDED = Messages.GetString("NGram.LATIN1_EXCLUDE");
        public const int N_GRAM = 3;
        public static Dictionary<char, char> cjk_map;

        private readonly char[] grams_;
        private int gramsLength;
        private bool capitalword_;

        /// <summary>
        /// Constructor.
        public NGram()
        {
            grams_ = new char[N_GRAM];
            grams_[0] = ' ';
            gramsLength = 1;
            capitalword_ = false;
        }

        /// <summary>
        /// Append a character into ngram buffer.
        /// </summary>
        /// <param name="ch"></param>
        public void AddChar(char ch)
        {
            ch = Normalize(ch);
            char lastchar = grams_[gramsLength - 1];
            if (lastchar == ' ')
            {
                grams_[0] = ' ';
                gramsLength = 1;
                capitalword_ = false;
                if (ch == ' ') return;
            }
            else if (gramsLength >= N_GRAM)
            {
                for (int i = 0; i < N_GRAM - 1; i++)
                {
                    grams_[i] = grams_[i + 1];
                    gramsLength = N_GRAM - 1;
                }
            }
            grams_[gramsLength++] = ch;

            if (char.IsUpper(ch))
            {
                if (char.IsUpper(lastchar)) capitalword_ = true;
            }
            else
            {
                capitalword_ = false;
            }
        }

        /// <summary>
        /// Get n-Gram
        /// </summary>
        /// <param name="n">Length of n-gram</param>
        /// <returns>n-Gram string (null if it is invalid)</returns>
        public string this[int n]
        {
            get
            {
                if (capitalword_) return null;
                int len = gramsLength;
                if (n < 1 || n > 3 || len < n) return null;
                if (n == 1)
                {
                    char ch = grams_[len - 1];
                    if (ch == ' ') return null;
                    return char.ToString(ch);
                }
                else
                {
                    return new string(grams_, len - n, n);
                }
            }
        }

        /// <summary>
        /// char Normalization
        /// </summary>
        /// <param name="ch"></param>
        /// <returns>Normalized character</returns>
        public static char Normalize(char ch)
        {
            if (CharIsInUnicodeRange(ch, UnicodeRanges.BasicLatin))
            {
                if (ch < 'A' || (ch < 'a' && ch > 'Z') || ch > 'z') ch = ' ';
            }
            else if (CharIsInUnicodeRange(ch, UnicodeRanges.Latin1Supplement))
            {
                if (LATIN1_EXCLUDED.IndexOf(ch) >= 0) ch = ' ';
            }
            else if (CharIsInUnicodeRange(ch, UnicodeRanges.LatinExtendedB))
            {
                // normalization for Romanian
                if (ch == '\u0219') ch = '\u015f';  // Small S with comma below => with cedilla
                if (ch == '\u021b') ch = '\u0163';  // Small T with comma below => with cedilla
            }
            else if (CharIsInUnicodeRange(ch, UnicodeRanges.GeneralPunctuation))
            {
                ch = ' ';
            }
            else if (CharIsInUnicodeRange(ch, UnicodeRanges.Arabic))
            {
                if (ch == '\u06cc') ch = '\u064a';  // Farsi yeh => Arabic yeh
            }
            else if (CharIsInUnicodeRange(ch, UnicodeRanges.LatinExtendedAdditional))
            {
                if (ch >= '\u1ea0') ch = '\u1ec3';
            }
            else if (CharIsInUnicodeRange(ch, UnicodeRanges.Hiragana))
            {
                ch = '\u3042';
            }
            else if (CharIsInUnicodeRange(ch, UnicodeRanges.Katakana))
            {
                ch = '\u30a2';
            }
            else if (CharIsInUnicodeRange(ch, UnicodeRanges.Bopomofo) || CharIsInUnicodeRange(ch, UnicodeRanges.BopomofoExtended))
            {
                ch = '\u3105';
            }
            else if (CharIsInUnicodeRange(ch, UnicodeRanges.CjkUnifiedIdeographs))
            {
                if (cjk_map.ContainsKey(ch)) ch = cjk_map[ch];
            }
            else if (CharIsInUnicodeRange(ch, UnicodeRanges.HangulSyllables))
            {
                ch = '\uac00';
            }
            return ch;
        }

        private static bool CharIsInUnicodeRange(char ch, UnicodeRange range)
        {
            return ch >= range.FirstCodePoint && ch < range.FirstCodePoint + range.Length;
        }

        /// <summary>
        /// Normalizer for Vietnamese.
        /// Normalize Alphabet + Diacritical Mark(U+03xx) into U+1Exx .
        /// </summary>
        /// <param name="text"></param>
        /// <returns>normalized text</returns>
        public static string normalize_vi(string text)
        {
            var matches = ALPHABET_WITH_DMARK.Matches(text);
            StringBuilder buf = new StringBuilder();
            foreach (Match m in matches)
            {
                int alphabet = TO_NORMALIZE_VI_CHARS.IndexOf(m.Groups[1].Value);
                int dmark = DMARK_CLASS.IndexOf(m.Groups[2].Value); // Diacritical Mark
                // TODO: m.appendReplacement(buf, NORMALIZED_VI_CHARS[dmark].Substring(alphabet, 1));
            }
            if (buf.Length == 0)
                return text;
            // TODO: m.appendTail(buf);
            return buf.ToString();
        }

        private static readonly string[] NORMALIZED_VI_CHARS = {
            Messages.GetString("NORMALIZED_VI_CHARS_0300"),
            Messages.GetString("NORMALIZED_VI_CHARS_0301"),
            Messages.GetString("NORMALIZED_VI_CHARS_0303"),
            Messages.GetString("NORMALIZED_VI_CHARS_0309"),
            Messages.GetString("NORMALIZED_VI_CHARS_0323") };
        private static readonly string TO_NORMALIZE_VI_CHARS = Messages.GetString("TO_NORMALIZE_VI_CHARS");
        private static readonly string DMARK_CLASS = Messages.GetString("DMARK_CLASS");
        private static readonly Regex ALPHABET_WITH_DMARK = new Regex("([" + TO_NORMALIZE_VI_CHARS + "])(["
                + DMARK_CLASS + "])");

        /// <summary>
        /// CJK Kanji Normalization Mapping
        /// </summary>
        static readonly string[] CJK_CLASS = {
            Messages.GetString("NGram.KANJI_1_0"),
            Messages.GetString("NGram.KANJI_1_2"),
            Messages.GetString("NGram.KANJI_1_4"),
            Messages.GetString("NGram.KANJI_1_8"),
            Messages.GetString("NGram.KANJI_1_11"),
            Messages.GetString("NGram.KANJI_1_12"),
            Messages.GetString("NGram.KANJI_1_13"),
            Messages.GetString("NGram.KANJI_1_14"),
            Messages.GetString("NGram.KANJI_1_16"),
            Messages.GetString("NGram.KANJI_1_18"),
            Messages.GetString("NGram.KANJI_1_22"),
            Messages.GetString("NGram.KANJI_1_27"),
            Messages.GetString("NGram.KANJI_1_29"),
            Messages.GetString("NGram.KANJI_1_31"),
            Messages.GetString("NGram.KANJI_1_35"),
            Messages.GetString("NGram.KANJI_2_0"),
            Messages.GetString("NGram.KANJI_2_1"),
            Messages.GetString("NGram.KANJI_2_4"),
            Messages.GetString("NGram.KANJI_2_9"),
            Messages.GetString("NGram.KANJI_2_10"),
            Messages.GetString("NGram.KANJI_2_11"),
            Messages.GetString("NGram.KANJI_2_12"),
            Messages.GetString("NGram.KANJI_2_13"),
            Messages.GetString("NGram.KANJI_2_15"),
            Messages.GetString("NGram.KANJI_2_16"),
            Messages.GetString("NGram.KANJI_2_18"),
            Messages.GetString("NGram.KANJI_2_21"),
            Messages.GetString("NGram.KANJI_2_22"),
            Messages.GetString("NGram.KANJI_2_23"),
            Messages.GetString("NGram.KANJI_2_28"),
            Messages.GetString("NGram.KANJI_2_29"),
            Messages.GetString("NGram.KANJI_2_30"),
            Messages.GetString("NGram.KANJI_2_31"),
            Messages.GetString("NGram.KANJI_2_32"),
            Messages.GetString("NGram.KANJI_2_35"),
            Messages.GetString("NGram.KANJI_2_36"),
            Messages.GetString("NGram.KANJI_2_37"),
            Messages.GetString("NGram.KANJI_2_38"),
            Messages.GetString("NGram.KANJI_3_1"),
            Messages.GetString("NGram.KANJI_3_2"),
            Messages.GetString("NGram.KANJI_3_3"),
            Messages.GetString("NGram.KANJI_3_4"),
            Messages.GetString("NGram.KANJI_3_5"),
            Messages.GetString("NGram.KANJI_3_8"),
            Messages.GetString("NGram.KANJI_3_9"),
            Messages.GetString("NGram.KANJI_3_11"),
            Messages.GetString("NGram.KANJI_3_12"),
            Messages.GetString("NGram.KANJI_3_13"),
            Messages.GetString("NGram.KANJI_3_15"),
            Messages.GetString("NGram.KANJI_3_16"),
            Messages.GetString("NGram.KANJI_3_18"),
            Messages.GetString("NGram.KANJI_3_19"),
            Messages.GetString("NGram.KANJI_3_22"),
            Messages.GetString("NGram.KANJI_3_23"),
            Messages.GetString("NGram.KANJI_3_27"),
            Messages.GetString("NGram.KANJI_3_29"),
            Messages.GetString("NGram.KANJI_3_30"),
            Messages.GetString("NGram.KANJI_3_31"),
            Messages.GetString("NGram.KANJI_3_32"),
            Messages.GetString("NGram.KANJI_3_35"),
            Messages.GetString("NGram.KANJI_3_36"),
            Messages.GetString("NGram.KANJI_3_37"),
            Messages.GetString("NGram.KANJI_3_38"),
            Messages.GetString("NGram.KANJI_4_0"),
            Messages.GetString("NGram.KANJI_4_9"),
            Messages.GetString("NGram.KANJI_4_10"),
            Messages.GetString("NGram.KANJI_4_16"),
            Messages.GetString("NGram.KANJI_4_17"),
            Messages.GetString("NGram.KANJI_4_18"),
            Messages.GetString("NGram.KANJI_4_22"),
            Messages.GetString("NGram.KANJI_4_24"),
            Messages.GetString("NGram.KANJI_4_28"),
            Messages.GetString("NGram.KANJI_4_34"),
            Messages.GetString("NGram.KANJI_4_39"),
            Messages.GetString("NGram.KANJI_5_10"),
            Messages.GetString("NGram.KANJI_5_11"),
            Messages.GetString("NGram.KANJI_5_12"),
            Messages.GetString("NGram.KANJI_5_13"),
            Messages.GetString("NGram.KANJI_5_14"),
            Messages.GetString("NGram.KANJI_5_18"),
            Messages.GetString("NGram.KANJI_5_26"),
            Messages.GetString("NGram.KANJI_5_29"),
            Messages.GetString("NGram.KANJI_5_34"),
            Messages.GetString("NGram.KANJI_5_39"),
            Messages.GetString("NGram.KANJI_6_0"),
            Messages.GetString("NGram.KANJI_6_3"),
            Messages.GetString("NGram.KANJI_6_9"),
            Messages.GetString("NGram.KANJI_6_10"),
            Messages.GetString("NGram.KANJI_6_11"),
            Messages.GetString("NGram.KANJI_6_12"),
            Messages.GetString("NGram.KANJI_6_16"),
            Messages.GetString("NGram.KANJI_6_18"),
            Messages.GetString("NGram.KANJI_6_20"),
            Messages.GetString("NGram.KANJI_6_21"),
            Messages.GetString("NGram.KANJI_6_22"),
            Messages.GetString("NGram.KANJI_6_23"),
            Messages.GetString("NGram.KANJI_6_25"),
            Messages.GetString("NGram.KANJI_6_28"),
            Messages.GetString("NGram.KANJI_6_29"),
            Messages.GetString("NGram.KANJI_6_30"),
            Messages.GetString("NGram.KANJI_6_32"),
            Messages.GetString("NGram.KANJI_6_34"),
            Messages.GetString("NGram.KANJI_6_35"),
            Messages.GetString("NGram.KANJI_6_37"),
            Messages.GetString("NGram.KANJI_6_39"),
            Messages.GetString("NGram.KANJI_7_0"),
            Messages.GetString("NGram.KANJI_7_3"),
            Messages.GetString("NGram.KANJI_7_6"),
            Messages.GetString("NGram.KANJI_7_7"),
            Messages.GetString("NGram.KANJI_7_9"),
            Messages.GetString("NGram.KANJI_7_11"),
            Messages.GetString("NGram.KANJI_7_12"),
            Messages.GetString("NGram.KANJI_7_13"),
            Messages.GetString("NGram.KANJI_7_16"),
            Messages.GetString("NGram.KANJI_7_18"),
            Messages.GetString("NGram.KANJI_7_19"),
            Messages.GetString("NGram.KANJI_7_20"),
            Messages.GetString("NGram.KANJI_7_21"),
            Messages.GetString("NGram.KANJI_7_23"),
            Messages.GetString("NGram.KANJI_7_25"),
            Messages.GetString("NGram.KANJI_7_28"),
            Messages.GetString("NGram.KANJI_7_29"),
            Messages.GetString("NGram.KANJI_7_32"),
            Messages.GetString("NGram.KANJI_7_33"),
            Messages.GetString("NGram.KANJI_7_35"),
            Messages.GetString("NGram.KANJI_7_37"),
        };

        static NGram()
        {
            cjk_map = new Dictionary<char, char>();
            foreach (string cjk_list in CJK_CLASS)
            {
                char representative = cjk_list[0];
                for (int i = 0; i < cjk_list.Length; ++i)
                {
                    cjk_map[cjk_list[i]] = representative;
                }
            }
        }
    }
}

