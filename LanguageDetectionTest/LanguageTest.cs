using LanguageDetection;
using NUnit.Framework;

namespace LanguageDetectionTest
{
    /// <summary>
    /// </summary>
    public class LanguageTest
    {
        /// <summary>
        /// Test method for {@link com.cybozu.labs.langdetect.Language#Language(java.lang.string, double)}.
        /// </summary>
        [Test]
        public void TestLanguage()
        {
            Language lang = new Language(null, 0);
            Assert.AreEqual(lang.Lang, null);
            Assert.AreEqual(lang.Prob, 0.0, 0.0001);
            Assert.AreEqual(lang.ToString(), "");

            Language lang2 = new Language("en", 1.0);
            Assert.AreEqual(lang2.Lang, "en");
            Assert.AreEqual(lang2.Prob, 1.0, 0.0001);
            Assert.AreEqual(lang2.ToString(), "en:1.0");
        }
    }
}
