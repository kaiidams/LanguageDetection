using LanguageDetection.Utils;
using NUnit.Framework;

namespace LanguageDetectionTest.Utils
{
    /**
    * 
    * @author Nakatani Shuyo
    *
    */
    public class LangProfileTest
    {

        /**
         * @throws java.lang.Exception
         */
        [SetUp]
        public void setUp()
        {
        }

        /**
         * @throws java.lang.Exception
         */
        [TearDown]
        public void tearDown()
        {
        }

        /**
         * Test method for {@link com.cybozu.labs.langdetect.util.LangProfile#LangProfile()}.
         */
        [Test]
        public void testLangProfile()
        {
            LangProfile profile = new LangProfile();
            Assert.AreEqual(profile.Name, null);
        }

        /**
         * Test method for {@link com.cybozu.labs.langdetect.util.LangProfile#LangProfile(java.lang.string)}.
         */
        [Test]
        public void testLangProfileStringInt()
        {
            LangProfile profile = new LangProfile("en");
            Assert.AreEqual(profile.Name, "en");
        }

        /**
         * Test method for {@link com.cybozu.labs.langdetect.util.LangProfile#add(java.lang.string)}.
         */
        [Test]
        public void testAdd()
        {
            LangProfile profile = new LangProfile("en");
            profile.Add("a");
            Assert.AreEqual((int)profile.Freq["a"], 1);
            profile.Add("a");
            Assert.AreEqual((int)profile.Freq["a"], 2);
            profile.OmitLessFreq();
        }


        /**
         * Illegal call test for {@link LangProfile#add(string)}
         */
        [Test]
        public void testAddIllegally1()
        {
            LangProfile profile = new LangProfile(); // Illegal ( available for only JSONIC ) but ignore  
            profile.Add("a"); // ignore
            Assert.AreEqual(profile.Freq["a"], null); // ignored
        }

        /**
         * Illegal call test for {@link LangProfile#add(string)}
         */
        [Test]
        public void testAddIllegally2()
        {
            LangProfile profile = new LangProfile("en");
            profile.Add("a");
            profile.Add("");  // Illegal (string's.Length of parameter must be between 1 and 3) but ignore
            profile.Add("abcd");  // as well
            Assert.AreEqual((int)profile.Freq["a"], 1);
            Assert.AreEqual(profile.Freq[""], null);     // ignored
            Assert.AreEqual(profile.Freq["abcd"], null); // ignored

        }

        /**
         * Test method for {@link com.cybozu.labs.langdetect.util.LangProfile#omitLessFreq()}.
         */
        [Test]
        public void testOmitLessFreq()
        {
            LangProfile profile = new LangProfile("en");
            string[] grams = "a b c \u3042 \u3044 \u3046 \u3048 \u304a \u304b \u304c \u304d \u304e \u304f".Split(" ");
            for (int i = 0; i < 5; ++i) foreach (string g in grams)
                {
                    profile.Add(g);
                }
            profile.Add("\u3050");

            Assert.AreEqual((int)profile.Freq["a"], 5);
            Assert.AreEqual((int)profile.Freq["\u3042"], 5);
            Assert.AreEqual((int)profile.Freq["\u3050"], 1);
            profile.OmitLessFreq();
            Assert.AreEqual(profile.Freq["a"], null); // omitted
            Assert.AreEqual((int)profile.Freq["\u3042"], 5);
            Assert.AreEqual(profile.Freq["\u3050"], null); // omitted
        }

        /**
         * Illegal call test for {@link com.cybozu.labs.langdetect.util.LangProfile#omitLessFreq()}.
         */
        [Test]
        public void testOmitLessFreqIllegally()
        {
            LangProfile profile = new LangProfile();
            profile.OmitLessFreq();  // ignore
        }
    }
}
