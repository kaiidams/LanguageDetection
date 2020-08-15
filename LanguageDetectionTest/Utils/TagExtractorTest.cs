




using LanguageDetection.Util;
using NUnit.Framework;
/**
* 
*//**
* @author Nakatani Shuyo
*
*/
public class TagExtractorTest {

    /**
     * @throws java.lang.Exception
     */
    [BeforeClass]
    public static void setUpBeforeClass() {
    }

    /**
     * @throws java.lang.Exception
     */
    [AfterClass]
    public static void tearDownAfterClass() {
    }

    /**
     * @throws java.lang.Exception
     */
    [SetUp]
    public void setUp() {
    }

    /**
     * @throws java.lang.Exception
     */
    [TearDown]
    public void tearDown() {
    }

    /**
     * Test method for {@link com.cybozu.labs.langdetect.util.TagExtractor#TagExtractor(java.lang.string, int)}.
     */
    [Test]
    public void testTagExtractor() {
        TagExtractor extractor = new TagExtractor(null, 0);
        Assert.AreEqual(extractor.target_, null);
        Assert.AreEqual(extractor.threshold_, 0);

        TagExtractor extractor2 = new TagExtractor("abstract", 10);
        Assert.AreEqual(extractor2.target_, "abstract");
        Assert.AreEqual(extractor2.threshold_, 10);
}

    /**
     * Test method for {@link com.cybozu.labs.langdetect.util.TagExtractor#setTag(java.lang.string)}.
     */
    [Test]
    public void testSetTag() {
        TagExtractor extractor = new TagExtractor(null, 0);
        extractor.SetTag("");
        Assert.AreEqual(extractor.tag_, "");
        extractor.SetTag(null);
        Assert.AreEqual(extractor.tag_, null);
    }

    /**
     * Test method for {@link com.cybozu.labs.langdetect.util.TagExtractor#add(java.lang.string)}.
     */
    [Test]
    public void testAdd() {
        TagExtractor extractor = new TagExtractor(null, 0);
        extractor.Add("");
        extractor.Add(null);    // ignore
    }

    /**
     * Test method for {@link com.cybozu.labs.langdetect.util.TagExtractor#closeTag(com.cybozu.labs.langdetect.util.LangProfile)}.
     */
    [Test]
    public void testCloseTag() {
        TagExtractor extractor = new TagExtractor(null, 0);
        extractor.CloseTag();    // ignore
    }

    
    /**
     * Scenario Test of extracting &lt;abstract&gt; tag from Wikipedia database.
     */
    [Test]
    public void testNormalScenario() {
        TagExtractor extractor = new TagExtractor("abstract", 10);
        Assert.AreEqual(extractor.Count(), 0);

        LangProfile profile = new LangProfile("en");

        // normal
        extractor.SetTag("abstract");
        extractor.Add("This is a sample text.");
        profile.Update(extractor.CloseTag());
        Assert.AreEqual(extractor.Count(), 1);
        Assert.AreEqual(profile.n_words[0], 17);  // Thisisasampletext
        Assert.AreEqual(profile.n_words[1], 22);  // _T, Th, hi, ...
        Assert.AreEqual(profile.n_words[2], 17);  // _Th, Thi, his, ...

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

    /**
     * Test method for {@link com.cybozu.labs.langdetect.util.TagExtractor#clear()}.
     */
    [Test]
    public void testClear() {
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
