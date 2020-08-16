namespace LanguageDetection
{
    /// <summary>
    /// {@link Language} is to store the detected language.
    /// {@link Detector#getProbabilities()} returns an {@link List} of {@link Language}s.
    /// <see>Detector#getProbabilities()</see>
    /// </summary>
    public struct Language
    {
        public string Lang { get; set; }
        public double Prob { get; set; }

        public Language(string lang, double prob)
        {
            Lang = lang;
            Prob = prob;
        }

        public override string ToString()
        {
            if (Lang == null) return "";
            return string.Format("{0}:{1:F4}", Lang, Prob);
        }
    }
}
