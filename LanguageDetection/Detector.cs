using LanguageDetection.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Unicode;

namespace LanguageDetection
{
    /**
    * {@link Detector} class is to detect language from specified text. 
    * Its instance is able to be constructed via the factory class {@link DetectorFactory}.
    * <p>
    * After appending a target text to the {@link Detector} instance with {@link #append(Reader)} or {@link #append(string)},
    * the detector provides the language detection results for target text via {@link #detect()} or {@link #getProbabilities()}.
    * {@link #detect()} method returns a single language name which has the highest probability.
    * {@link #getProbabilities()} methods returns a list of multiple languages and their probabilities.
    * <p>  
    * The detector has some parameters for language detection.
    * See {@link #setAlpha(double)}, {@link #setMaxTextLength(int)} and {@link #setPriorMap(Dictionary)}.
    * 
    * <pre>
    * import java.util.List;
    * import com.cybozu.labs.langdetect.Detector;
    * import com.cybozu.labs.langdetect.DetectorFactory;
    * import com.cybozu.labs.langdetect.Language;
    * 
    * class LangDetectSample {
    *     public void init(string profileDirectory) {
    *         DetectorFactory.loadProfile(profileDirectory);
    *     }
    *     public string detect(string text) {
    *         Detector detector = DetectorFactory.create();
    *         detector.Append(text);
    *         return detector.detect();
    *     }
    *     public List<Language> detectLangs(string text) {
    *         Detector detector = DetectorFactory.create();
    *         detector.Append(text);
    *         return detector.GetProbabilities();
    *     }
    * }
    * </pre>
    * 
    * <ul>
    * <li>4x faster improvement based on Elmer Garduno's code. Thanks!</li>
    * </ul>
    * 
    * @author Nakatani Shuyo
    * @see DetectorFactory
*/
    public class Detector
    {
        private const double ALPHA_DEFAULT = 0.5;
        private const double ALPHA_WIDTH = 0.05;

        private const int ITERATION_LIMIT = 1000;
        private const double PROB_THRESHOLD = 0.1;
        private const double CONV_THRESHOLD = 0.99999;
        private const int BASE_FREQ = 10000;
        private const string UNKNOWN_LANG = "unknown";

        private readonly Regex URL_REGEX = new Regex("https?://[-_.?&~;+=/#0-9A-Za-z]{1,2076}");
        private readonly Regex MAIL_REGEX = new Regex("[-_.0-9A-Za-z]{1,64}@[-_0-9A-Za-z]{1,255}[-_.0-9A-Za-z]{1,255}");

        private readonly Dictionary<string, double[]> wordLangProbMap;
        private readonly List<string> langlist;

        private StringBuilder text;
        private double[] langprob = null;

        private double alpha = ALPHA_DEFAULT;
        private int n_trial = 7;
        private int max_text_length = 10000;
        private double[] priorMap = null;
        private bool verbose = false;
        private int? seed = null;

        /**
         * Constructor.
         * Detector instance can be constructed via {@link DetectorFactory#create()}.
         * @param factory {@link DetectorFactory} instance (only DetectorFactory inside)
         */
        public Detector(DetectorFactory factory)
        {
            this.wordLangProbMap = factory.wordLangProbMap;
            this.langlist = factory.langlist;
            this.text = new StringBuilder();
            this.seed = factory.seed;
        }

        /**
         * Set Verbose Mode(use for debug).
         */
        public void SetVerbose()
        {
            this.verbose = true;
        }

        /**
         * Set smoothing parameter.
         * The default value is 0.5(i.e. Expected Likelihood Estimate).
         * @param alpha the smoothing parameter
         */
        public void SetAlpha(double alpha)
        {
            this.alpha = alpha;
        }

        /**
         * Set prior information about language probabilities.
         * @param priorMap the priorMap to set
         * @throws LangDetectException 
         */
        public void SetPriorMap(Dictionary<string, double> priorMap)
        {
            this.priorMap = new double[langlist.Count];
            double sump = 0;
            for (int i = 0; i < this.priorMap.Length; ++i)
            {
                string lang = langlist[i];
                if (priorMap.ContainsKey(lang))
                {
                    double p = priorMap[lang];
                    if (p < 0) throw new LangDetectException(ErrorCode.InitParamError, "Prior probability must be non-negative.");
                    this.priorMap[i] = p;
                    sump += p;
                }
            }
            if (sump <= 0) throw new LangDetectException(ErrorCode.InitParamError, "More one of prior probability must be non-zero.");
            for (int i = 0; i < this.priorMap.Length; ++i) this.priorMap[i] /= sump;
        }

        /**
         * Specify max size of target text to use for language detection.
         * The default value is 10000(10KB).
         * @param max_text_length the max_text_length to set
         */
        public void SetMaxTextLength(int max_text_length)
        {
            this.max_text_length = max_text_length;
        }


        /**
         * Append the target text for language detection.
         * This method read the text from specified input reader.
         * If the total size of target text exceeds the limit size specified by {@link Detector#setMaxTextLength(int)},
         * the rest is cut down.
         * 
         * @param reader the input reader (BufferedReader as usual)
         * @throws IOException Can't read the reader.
         */
        public void Append(StreamReader reader)
        {
            char[] buf = new char[max_text_length / 2];
            while (text.Length < max_text_length && !reader.EndOfStream)
            {
                int length = reader.Read(buf, 0, buf.Length);
                Append(new string(buf, 0, length));
            }
        }

        /**
         * Append the target text for language detection.
         * If the total size of target text exceeds the limit size specified by {@link Detector#setMaxTextLength(int)},
         * the rest is cut down.
         * 
         * @param text the target text to append
         */
        public void Append(string text)
        {
            text = URL_REGEX.Replace(text, " ");
            text = MAIL_REGEX.Replace(text, " ");
            text = NGram.normalize_vi(text);
            char pre = '\0';
            for (int i = 0; i < text.Length && i < max_text_length; ++i)
            {
                char c = text[i];
                if (c != ' ' || pre != ' ') this.text.Append(c);
                pre = c;
            }
        }

        /**
         * Cleaning text to detect
         * (eliminate URL, e-mail address and Latin sentence if it is not written in Latin alphabet)
         */
        private void CleaningText()
        {
            int latinCount = 0, nonLatinCount = 0;
            for (int i = 0; i < text.Length; ++i)
            {
                char c = text[i];
                if (c <= 'z' && c >= 'A')
                {
                    ++latinCount;
                }
                else if (c >= '\u0300' && (c < UnicodeRanges.LatinExtendedAdditional.FirstCodePoint || c >= (UnicodeRanges.LatinExtendedAdditional.Length + UnicodeRanges.LatinExtendedAdditional.FirstCodePoint)))
                {
                    ++nonLatinCount;
                }
            }
            if (latinCount * 2 < nonLatinCount)
            {
                StringBuilder textWithoutLatin = new StringBuilder();
                for (int i = 0; i < text.Length; ++i)
                {
                    char c = text[i];
                    if (c > 'z' || c < 'A') textWithoutLatin.Append(c);
                }
                text = textWithoutLatin;
            }

        }

        /**
         * Detect language of the target text and return the language name which has the highest probability.
         * @return detected language name which has most probability.
         * @throws LangDetectException 
         *  code = ErrorCode.CantDetectError : Can't detect because of no valid features in text
         */
        public string Detect()
        {
            List<Language> probabilities = GetProbabilities();
            if (probabilities.Count > 0) return probabilities[0].Lang;
            return UNKNOWN_LANG;
        }

        /**
         * Get language candidates which have high probabilities
         * @return possible languages list (whose probabilities are over PROB_THRESHOLD, ordered by probabilities descendently
         * @throws LangDetectException 
         *  code = ErrorCode.CantDetectError : Can't detect because of no valid features in text
         */
        public List<Language> GetProbabilities()
        {
            if (langprob == null) DetectBlock();

            List<Language> list = SortProbability(langprob);
            return list;
        }

        private double SampleGaussian(Random rand)
        {
            double angle = Math.PI * rand.NextDouble();
            double distance = Math.Sqrt(-2.0 * Math.Log(rand.NextDouble()));
            return Math.Cos(angle) * distance;
        }

        /**
         * @throws LangDetectException 
         * 
         */
        private void DetectBlock()
        {
            CleaningText();
            List<string> ngrams = ExtractNGrams();
            if (ngrams.Count == 0)
                throw new LangDetectException(ErrorCode.CantDetectError, "no features in text");

            langprob = new double[langlist.Count];

            var rand = seed != null ? new Random((int)seed) : new Random();
            for (int t = 0; t < n_trial; ++t)
            {
                double[] prob = InitProbability();
                double alpha = this.alpha + SampleGaussian(rand) * ALPHA_WIDTH;

                for (int i = 0; ; ++i)
                {
                    int r = rand.Next(ngrams.Count);
                    UpdateLangProb(prob, ngrams[r], alpha);
                    if (i % 5 == 0)
                    {
                        if (NormalizeProb(prob) > CONV_THRESHOLD || i >= ITERATION_LIMIT) break;
                        if (verbose) Console.WriteLine("> " + SortProbability(prob));
                    }
                }
                for (int j = 0; j < langprob.Length; ++j) langprob[j] += prob[j] / n_trial;
                if (verbose) Console.WriteLine("==> " + SortProbability(prob));
            }
        }

        /**
         * Initialize the map of language probabilities.
         * If there is the specified prior map, use it as initial map.
         * @return initialized map of language probabilities
         */
        private double[] InitProbability()
        {
            double[] prob = new double[langlist.Count];
            if (priorMap != null)
            {
                for (int i = 0; i < prob.Length; ++i) prob[i] = priorMap[i];
            }
            else
            {
                for (int i = 0; i < prob.Length; ++i) prob[i] = 1.0 / langlist.Count;
            }
            return prob;
        }

        /**
         * Extract n-grams from target text
         * @return n-grams list
         */
        private List<string> ExtractNGrams()
        {
            List<string> list = new List<string>();
            NGram ngram = new NGram();
            for (int i = 0; i < text.Length; ++i)
            {
                ngram.AddChar(text[i]);
                for (int n = 1; n <= NGram.N_GRAM; ++n)
                {
                    string w = ngram[n];
                    if (w != null && wordLangProbMap.ContainsKey(w)) list.Add(w);
                }
            }
            return list;
        }

        /**
         * update language probabilities with N-gram string(N=1,2,3)
         * @param word N-gram string
         */
        private bool UpdateLangProb(double[] prob, string word, double alpha)
        {
            if (word == null || !wordLangProbMap.ContainsKey(word)) return false;

            double[] langProbMap = wordLangProbMap[word];
            if (verbose) Console.WriteLine(word + "(" + UnicodeEncode(word) + "):" + WordProbToString(langProbMap));

            double weight = alpha / BASE_FREQ;
            for (int i = 0; i < prob.Length; ++i)
            {
                prob[i] *= weight + langProbMap[i];
            }
            return true;
        }

        private string WordProbToString(double[] prob)
        {
            var sb = new StringBuilder();
            for (int j = 0; j < prob.Length; ++j)
            {
                double p = prob[j];
                if (p >= 0.00001)
                {
                    sb.AppendFormat(" {0}:{1:F5}", langlist[j], p);
                }
            }
            string str = sb.ToString();
            return str;
        }

        /**
         * normalize probabilities and check convergence by the maximun probability
         * @return maximum of probabilities
         */
        static private double NormalizeProb(double[] prob)
        {
            double maxp = 0, sump = 0;
            for (int i = 0; i < prob.Length; ++i) sump += prob[i];
            for (int i = 0; i < prob.Length; ++i)
            {
                double p = prob[i] / sump;
                if (maxp < p) maxp = p;
                prob[i] = p;
            }
            return maxp;
        }

        /**
         * @param probabilities Dictionary
         * @return lanugage candidates order by probabilities descendently
         */
        private List<Language> SortProbability(double[] prob)
        {
            List<Language> list = new List<Language>();
            for (int j = 0; j < prob.Length; ++j)
            {
                double p = prob[j];
                if (p > PROB_THRESHOLD)
                {
                    for (int i = 0; i <= list.Count; ++i)
                    {
                        if (i == list.Count || list[i].Prob < p)
                        {
                            list.Insert(i, new Language(langlist[j], p));
                            break;
                        }
                    }
                }
            }
            return list;
        }

        /**
         * unicode encoding (for verbose mode)
         * @param word
         * @return
         */
        static private string UnicodeEncode(string word)
        {
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < word.Length; ++i)
            {
                char ch = word[i];
                if (ch >= '\u0080')
                {
                    string st = ((int)ch).ToString("X4");
                    buf.Append("\\u");
                    buf.Append(st);
                }
                else
                {
                    buf.Append(ch);
                }
            }
            return buf.ToString();
        }
    }
}
