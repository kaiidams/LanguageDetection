using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Text.Json;
using LanguageDetection.Utils;
using LanguageDetection;
using System.Linq;

namespace LanguageDetectionCommand
{
    /// <summary>
    /// LangDetect Command Line Interface
    /// <p>
    /// This is a command line interface of Language Detection Library "LandDetect".
    /// </summary>
    public class Program
    {
        /// <summary>smoothing default parameter (ELE)</summary> 
        private const double DEFAULT_ALPHA = 0.5;

        /// <summary>for Command line easy parser</summary>
        private Dictionary<string, string> opt_with_value = new Dictionary<string, string>();
        private Dictionary<string, string> values = new Dictionary<string, string>();
        private HashSet<string> opt_without_value = new HashSet<string>();
        private List<string> arglist = new List<string>();

        /// <summary>
        /// Command line easy parser
        /// </summary>
        /// <param name="args">command line arguments</param>
        private void Parse(string[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                if (opt_with_value.ContainsKey(args[i]))
                {
                    string key = opt_with_value[args[i]];
                    values[key] = args[i + 1];
                    ++i;
                }
                else if (args[i].StartsWith("-"))
                {
                    opt_without_value.Add(args[i]);
                }
                else
                {
                    arglist.Add(args[i]);
                }
            }
        }

        private void AddOpt(string opt, string key, string value)
        {
            opt_with_value[opt] = key;
            values[key] = value;
        }

        private string Get(string key)
        {
            return values[key];
        }

        private long? GetLong(string key)
        {
            string value = values[key];
            if (value == null) return null;
            try
            {
                return long.Parse(value);
            }
            catch (FormatException)
            {
                return null;
            }
        }
        private double GetDouble(string key, double defaultValue)
        {
            try
            {
                return double.Parse(values[key]);
            }
            catch (FormatException)
            {
                return defaultValue;
            }
        }

        private bool HasOpt(string opt)
        {
            return opt_without_value.Contains(opt);
        }


        /// <summary>
        /// File search (easy glob)
        /// </summary>
        /// <param name="directory">directory path</param>
        /// <param name="pattern">searching file pattern with regular representation</param>
        /// <returns>matched file</returns>
        private string SearchFile(string directory, string pattern)
        {
            var rx = new Regex(pattern);
            foreach (string file in Directory.GetFiles(directory))
            {
                if (rx.IsMatch(file)) return file;
            }
            return null;
        }


        /// <summary>
        /// load profiles
        /// </summary>
        /// <returns>false if load success</returns>
        private bool LoadProfile()
        {
            string profileDirectory = Get("directory");
            try
            {
                DetectorFactory.LoadProfile(profileDirectory);
                long? seed = GetLong("seed");
                if (seed != null) DetectorFactory.SetSeed((int)seed);
                return false;
            }
            catch (LangDetectException e)
            {
                Console.Error.WriteLine("ERROR: " + e);
                return true;
            }
        }

        /// <summary>
        /// Generate Language Profile from Wikipedia Abstract Database File
        /// <pre>
        /// usage: --genprofile -d [abstracts directory] [language names]
        /// </pre>
        /// </summary>
        public void GenerateProfile()
        {
            string directory = Get("directory");
            foreach (string lang in arglist)
            {
                string file = SearchFile(directory, lang + "wiki-.*-abstract\\.xml.*");
                if (file == null)
                {
                    Console.Error.WriteLine("Not Found abstract xml : lang = " + lang);
                    continue;
                }

                FileStream os = null;
                try
                {
                    LangProfile profile = GenProfile.LoadFromWikipediaAbstract(lang, file);
                    profile.OmitLessFreq();

                    string profile_path = Get("directory") + "/profiles/" + lang;
                    File.WriteAllText(profile_path, JsonSerializer.Serialize(profile));
                }
                catch (NotSupportedException e)
                {
                    Debug.WriteLine(e);
                }
                catch (IOException e)
                {
                    Debug.WriteLine(e);
                }
                catch (LangDetectException e)
                {
                    Debug.WriteLine(e);
                }
                finally
                {
                    try
                    {
                        if (os != null) os.Close();
                    }
                    catch (IOException e) { }
                }
            }
        }

        /// <summary>
        /// Generate Language Profile from Text File
        /// <pre>
        /// usage: --genprofile-text -l [language code] [text file path]
        /// </pre>
        /// </summary>
        private void generateProfileFromText()
        {
            if (arglist.Count != 1)
            {
                Console.Error.WriteLine("Need to specify text file path");
                return;
            }
            string file = arglist[0];
            if (!File.Exists(file))
            {
                Console.Error.WriteLine("Need to specify existing text file path");
                return;
            }

            string lang = Get("lang");
            if (lang == null)
            {
                Console.Error.WriteLine("Need to specify langage code(-l)");
                return;
            }

            FileStream os = null;
            try
            {
                LangProfile profile = GenProfile.LoadFromText(lang, file);
                profile.OmitLessFreq();

                string profile_path = lang;
                File.WriteAllText(profile_path, JsonSerializer.Serialize(profile));
            }
            catch (NotSupportedException e)
            {
                Debug.WriteLine(e);
            }
            catch (IOException e)
            {
                Debug.WriteLine(e);
            }
            catch (LangDetectException e)
            {
                Debug.WriteLine(e);
            }
            finally
            {
                try
                {
                    if (os != null) os.Close();
                }
                catch (IOException e) { }
            }
        }

        /// <summary>
        /// Language detection test for each file (--detectlang option)
        /// <pre>
        /// usage: --detectlang -d [profile directory] -a [alpha] -s [seed] [test file(s)]
        /// </pre>
        public void DetectLang()
        {
            if (LoadProfile()) return;
            foreach (string filename in arglist)
            {
                StreamReader strm = null;
                try
                {
                    strm = new StreamReader(File.OpenRead(filename));

                    Detector detector = DetectorFactory.Create(GetDouble("alpha", DEFAULT_ALPHA));
                    if (HasOpt("--debug")) detector.SetVerbose();
                    detector.Append(strm);
                    var probs = string.Join(" ", detector.GetProbabilities().Select((lang) => lang.ToString()));
                    Console.WriteLine("{0}: {1}", filename, probs);
                }
                catch (IOException e)
                {
                    Debug.WriteLine(e);
                }
                catch (LangDetectException e)
                {
                    Debug.WriteLine(e);
                }
                finally
                {
                    try
                    {
                        if (strm != null) strm.Close();
                    }
                    catch (IOException e) { }
                }
            }
        }

        /// <summary>
        /// Batch Test of Language Detection (--batchtest option)
        /// <pre>
        /// usage: --batchtest -d [profile directory] -a [alpha] -s [seed] [test data(s)]
        /// </pre>
        /// The format of test data(s):
        /// <pre>
        ///   [correct language name]\t[text body for test]\n
        /// </pre>
        public void BatchTest()
        {
            if (LoadProfile()) return;
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (string filename in arglist)
            {
                StreamReader strm = null;
                try
                {
                    strm = new StreamReader(File.OpenRead(filename));
                    while (!strm.EndOfStream)
                    {
                        string line = strm.ReadLine();
                        int idx = line.IndexOf('\t');
                        if (idx <= 0) continue;
                        string correctLang = line.Substring(0, idx);
                        string text = line.Substring(idx + 1);

                        Detector detector = DetectorFactory.Create(GetDouble("alpha", DEFAULT_ALPHA));
                        detector.Append(text);
                        string lang = "";
                        try
                        {
                            lang = detector.Detect();
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e);
                        }
                        if (!result.ContainsKey(correctLang)) result[correctLang] = new List<string>();
                        result[correctLang].Add(lang);
                        if (HasOpt("--debug")) Console.WriteLine(correctLang + "," + lang + "," + (text.Length > 100 ? text.Substring(0, 100) : text));
                    }

                }
                catch (IOException e)
                {
                    Debug.WriteLine(e);
                }
                catch (LangDetectException e)
                {
                    Debug.WriteLine(e);
                }
                finally
                {
                    try
                    {
                        if (strm != null) strm.Close();
                    }
                    catch (IOException e) { }
                }

                List<string> langlist = new List<string>(result.Keys);
                langlist.Sort();

                int totalCount = 0, totalCorrect = 0;
                foreach (string lang in langlist)
                {
                    Dictionary<string, int> resultCount = new Dictionary<string, int>();
                    int count = 0;
                    List<string> list = result[lang];
                    foreach (string detectedLang in list)
                    {
                        ++count;
                        if (resultCount.ContainsKey(detectedLang))
                        {
                            resultCount[detectedLang] = resultCount[detectedLang] + 1;
                        }
                        else
                        {
                            resultCount[detectedLang] = 1;
                        }
                    }
                    int correct = resultCount.ContainsKey(lang) ? resultCount[lang] : 0;
                    double rate = correct / (double)count;
                    Console.WriteLine(string.Format("%s (%d/%d=%.2f): %s", lang, correct, count, rate, resultCount));
                    totalCorrect += correct;
                    totalCount += count;
                }
                Console.WriteLine(string.Format("total: %d/%d = %.3f", totalCorrect, totalCount, totalCorrect / (double)totalCount));

            }

        }

        /// <summary>
        /// Command Line Interface
        /// </summary>
        /// <param name="args">command line arguments</param>
        public static void Main(string[] args)
        {
            Program command = new Program();
            command.AddOpt("-d", "directory", "./");
            command.AddOpt("-a", "alpha", "" + DEFAULT_ALPHA);
            command.AddOpt("-s", "seed", null);
            command.AddOpt("-l", "lang", null);
            command.Parse(args);

            if (command.HasOpt("--genprofile"))
            {
                command.GenerateProfile();
            }
            else if (command.HasOpt("--genprofile-text"))
            {
                command.generateProfileFromText();
            }
            else if (command.HasOpt("--detectlang"))
            {
                command.DetectLang();
            }
            else if (command.HasOpt("--batchtest"))
            {
                command.BatchTest();
            }
        }
    }
}

