




using NUnit.Framework;
/**
* 
*//**
* @author Nakatani Shuyo
*
*/
public class LanguageTest {

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
     * Test method for {@link com.cybozu.labs.langdetect.Language#Language(java.lang.string, double)}.
     */
    [Test]
    public void testLanguage() {
        Language lang = new Language(null, 0);
        Assert.AreEqual(lang.lang, null);
        Assert.AreEqual(lang.prob, 0.0, 0.0001);
        Assert.AreEqual(lang.ToString(), "");
        
        Language lang2 = new Language("en", 1.0);
        Assert.AreEqual(lang2.lang, "en");
        Assert.AreEqual(lang2.prob, 1.0, 0.0001);
        Assert.AreEqual(lang2.ToString(), "en:1.0");
        
    }

}
