using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System;
using System.Collections.ObjectModel;
using LanguageDetection.Utils;

namespace LanguageDetection
{
    /// <summary>
    /// Language Detector Factory Class
    /// This class manages an initialization and constructions of {@link Detector}. 
    /// Before using language detection library, 
    /// load profiles with {@link DetectorFactory#loadProfile(string)} method
    /// and set initialization parameters.
    /// When the language detection,
    /// construct Detector instance via {@link DetectorFactory#create()}.
    /// See also {@link Detector}'s sample code.
    /// <ul>
    /// <li>4x faster improvement based on Elmer Garduno's code. Thanks!</li>
    /// </ul>
    /// <see>Detector</see>
    /// </summary>
    public class DetectorFactory
    {
        public Dictionary<string, double[]> wordLangProbMap;
        public List<string> langlist;
        public int? seed = null;
        private DetectorFactory()
        {
            wordLangProbMap = new Dictionary<string, double[]>();
            langlist = new List<string>();
        }
        static private DetectorFactory instance_ = new DetectorFactory();

        /// <summary>
        /// Load profiles from specified directory.
        /// This method must be called once before language detection.
        /// </summary>
        /// <param name="profileDirectory">profile directory path</param>
        /// <exception>LangDetectException  Can't open profiles(error code = {@link ErrorCode#FileLoadError})</exception>
        ///                              or profile's format is wrong (error code = {@link ErrorCode#FormatError})
        public static void LoadProfile(string profileDirectory)
        {
            string[] listFiles = Directory.GetFiles(profileDirectory);
            if (listFiles == null)
                throw new LangDetectException(ErrorCode.NeedLoadProfileError, "Not found profile: " + profileDirectory);

            int langsize = listFiles.Length, index = 0;
            foreach (string file in listFiles)
            {
                if (Path.GetFileName(file).StartsWith(".") || !File.Exists(file)) continue;
                try
                {
                    LangProfile profile = JsonSerializer.Deserialize<LangProfile>(File.ReadAllText(file));
                    AddProfile(profile, index, langsize);
                    ++index;
                }
                catch (NotSupportedException e)
                {
                    throw new LangDetectException(ErrorCode.FormatError, "profile format error in '" + file + "'");
                }
                catch (IOException e)
                {
                    throw new LangDetectException(ErrorCode.FileLoadError, "can't open '" + file + "'");
                }
            }
        }

        /// <summary>
        /// Load profiles from specified directory.
        /// This method must be called once before language detection.
        /// </summary>
        /// <param name="profileDirectory">profile directory path</param>
        /// <exception>LangDetectException  Can't open profiles(error code = {@link ErrorCode#FileLoadError})</exception>
        ///                              or profile's format is wrong (error code = {@link ErrorCode#FormatError})
        public static void LoadProfile(IList<string> json_profiles)
        {
            int index = 0;
            int langsize = json_profiles.Count;
            if (langsize < 2)
                throw new LangDetectException(ErrorCode.NeedLoadProfileError, "Need more than 2 profiles");

            foreach (string json in json_profiles)
            {
                try
                {

                    LangProfile profile = JsonSerializer.Deserialize<LangProfile>(json);
                    AddProfile(profile, index, langsize);
                    ++index;
                }
                catch (NotSupportedException e)
                {
                    throw new LangDetectException(ErrorCode.FormatError, "profile format error");
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="langsize"></param>
        /// <param name="index"></param>
        /// <exception>LangDetectException </exception>
        static public /*internal*/ void AddProfile(LangProfile profile, int index, int langsize)
        {
            string lang = profile.Name;
            if (instance_.langlist.Contains(lang))
            {
                throw new LangDetectException(ErrorCode.DuplicateLangError, "duplicate the same language profile");
            }
            instance_.langlist.Add(lang);
            foreach (string word in profile.Freq.Keys)
            {
                if (!instance_.wordLangProbMap.ContainsKey(word))
                {
                    instance_.wordLangProbMap[word] = new double[langsize];
                }
                int length = word.Length;
                if (length >= 1 && length <= 3)
                {
                    double prob = ((double)profile.Freq[word]) / profile.N_Words[length - 1];
                    instance_.wordLangProbMap[word][index] = prob;
                }
            }
        }

        /// <summary>
        /// Clear loaded language profiles (reinitialization to be available)
        static public void Clear()
        {
            instance_.langlist.Clear();
            instance_.wordLangProbMap.Clear();
        }

        /// <summary>
        /// Construct Detector instance
        /// </summary>
        /// <returns>Detector instance</returns>
        /// <exception>LangDetectException </exception>
        static public Detector Create()
        {
            return CreateDetector();
        }

        /// <summary>
        /// Construct Detector instance with smoothing parameter 
        /// </summary>
        /// <param name="alpha">smoothing parameter (default value = 0.5)</param>
        /// <returns>Detector instance</returns>
        /// <exception>LangDetectException </exception>

        public static Detector Create(double alpha)
        {
            Detector detector = CreateDetector();
            detector.SetAlpha(alpha);
            return detector;
        }

        static private Detector CreateDetector()
        {
            if (instance_.langlist.Count == 0)
                throw new LangDetectException(ErrorCode.NeedLoadProfileError, "need to load profiles");
            Detector detector = new Detector(instance_);
            return detector;
        }

        public static void SetSeed(int seed)
        {
            instance_.seed = seed;
        }

        public static IList<string> GetLangList()
        {
            return new ReadOnlyCollection<string>(instance_.langlist);
        }
    }
}
