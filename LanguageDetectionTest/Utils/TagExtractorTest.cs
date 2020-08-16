using LanguageDetection.Utils;
using NUnit.Framework;

namespace LanguageDetectionTest.Utils
{
    /// <summary>
    /// </summary>
    public class TagExtractorTest
    {
        /// <summary>
        /// Test method for {@link com.cybozu.labs.langdetect.util.TagExtractor#TagExtractor(java.lang.string, int)}.
        /// </summary>
        [Test]
        public void testTagExtractor()
        {
            TagExtractor extractor = new TagExtractor(null, 0);
            Assert.AreEqual(extractor.target_, null);
            Assert.AreEqual(extractor.threshold_, 0);

            TagExtractor extractor2 = new TagExtractor("abstract", 10);
            Assert.AreEqual(extractor2.target_, "abstract");
            Assert.AreEqual(extractor2.threshold_, 10);
        }

        /// <summary>
        /// Test method for {@link com.cybozu.labs.langdetect.util.TagExtractor#setTag(java.lang.string)}.
        /// </summary>
        [Test]
        public void testSetTag()
        {
            TagExtractor extractor = new TagExtractor(null, 0);
            extractor.SetTag("");
            Assert.AreEqual(extractor.tag_, "");
            extractor.SetTag(null);
            Assert.AreEqual(extractor.tag_, null);
        }

        /// <summary>
        /// Test method for {@link com.cybozu.labs.langdetect.util.TagExtractor#add(java.lang.string)}.
        /// </summary>
        [Test]
        public void testAdd()
        {
            TagExtractor extractor = new TagExtractor(null, 0);
            extractor.Add("");
            extractor.Add(null);    // ignore
        }

        /// <summary>
        /// Test method for {@link com.cybozu.labs.langdetect.util.TagExtractor#closeTag(com.cybozu.labs.langdetect.util.LangProfile)}.
        /// </summary>
        [Test]
        public void testCloseTag()
        {
            TagExtractor extractor = new TagExtractor(null, 0);
            extractor.CloseTag();    // ignore
        }


        /// <summary>
        /// Scenario Test of extracting &lt;abstract&gt; tag from Wikipedia database.
        /// </summary>
        [Test]
        public void testNormalScenario()
        {
            TagExtractor extractor = new TagExtractor("abstract", 10);
            Assert.AreEqual(extractor.Count(), 0);

            LangProfile profile = new LangProfile("en");

            // normal
            extractor.SetTag("abstract");
            extractor.Add("This is a sample text.");
            profile.Update(extractor.CloseTag());
            Assert.AreEqual(extractor.Count(), 1);
            Assert.AreEqual(profile.N_Words[0], 17);  // Thisisasampletext
            Assert.AreEqual(profile.N_Words[1], 22);  // _T, Th, hi, ...
            Assert.AreEqual(profile.N_Words[2], 17);  // _Th, Thi, his, ...

            // too short
            extractor.SetTag("abstract");
            extractor.Add("sample");
            profile.Update(extractor.CloseTag());
            Assert.AreEqual(extractor.Count(), 1);

            // other tags
            extractor.SetTag("div");
            extractor.Add("This is a sample text which is enough long.");
            profile.Update(extractor.CloseTag());
            Assert.AreEqual(extractor.Count(), 1);
        }

        /// <summary>
        /// Test method for {@link com.cybozu.labs.langdetect.util.TagExtractor#clear()}.
        /// </summary>
        [Test]
        public void testClear()
        {
            TagExtractor extractor = new TagExtractor("abstract", 10);
            extractor.SetTag("abstract");
            extractor.Add("This is a sample text.");
            Assert.AreEqual(extractor.buf_.ToString(), "This is a sample text.");
            Assert.AreEqual(extractor.tag_, "abstract");
            extractor.Clear();
            Assert.AreEqual(extractor.buf_.ToString(), "");
            Assert.AreEqual(extractor.tag_, null);
        }
    }
}

