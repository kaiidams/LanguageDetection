




using LanguageDetection.Utils;
using NUnit.Framework;
/**
* 
*//**
* @author Nakatani Shuyo
*
*/
public class NGramTest {

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
     * Test method for constants
     */
    [Test]
    public void testConstants()
    {
        Assert.IsInstanceOf<int>(NGram.N_GRAM);
        Assert.AreEqual(NGram.N_GRAM, 3);
    }

    /**
     * Test method for {@link NGram#normalize(char)} with Latin characters
     */
    [Test]
    public void testNormalizeWithLatin() {
        Assert.AreEqual(NGram.normalize('\u0000'), ' ');
        Assert.AreEqual(NGram.normalize('\u0009'), ' ');
        Assert.AreEqual(NGram.normalize('\u0020'), ' ');
        Assert.AreEqual(NGram.normalize('\u0030'), ' ');
        Assert.AreEqual(NGram.normalize('\u0040'), ' ');
        Assert.AreEqual(NGram.normalize('\u0041'), '\u0041');
        Assert.AreEqual(NGram.normalize('\u005a'), '\u005a');
        Assert.AreEqual(NGram.normalize('\u005b'), ' ');
        Assert.AreEqual(NGram.normalize('\u0060'), ' ');
        Assert.AreEqual(NGram.normalize('\u0061'), '\u0061');
        Assert.AreEqual(NGram.normalize('\u007a'), '\u007a');
        Assert.AreEqual(NGram.normalize('\u007b'), ' ');
        Assert.AreEqual(NGram.normalize('\u007f'), ' ');
        Assert.AreEqual(NGram.normalize('\u0080'), '\u0080');
        Assert.AreEqual(NGram.normalize('\u00a0'), ' ');
        Assert.AreEqual(NGram.normalize('\u00a1'), '\u00a1');
    }

    /**
     * Test method for {@link NGram#normalize(char)} with CJK Kanji characters
     */
    [Test]
    public void testNormalizeWithCJKKanji() {
        Assert.AreEqual(NGram.normalize('\u4E00'), '\u4E00');
        Assert.AreEqual(NGram.normalize('\u4E01'), '\u4E01');
        Assert.AreEqual(NGram.normalize('\u4E02'), '\u4E02');
        Assert.AreEqual(NGram.normalize('\u4E03'), '\u4E01');
        Assert.AreEqual(NGram.normalize('\u4E04'), '\u4E04');
        Assert.AreEqual(NGram.normalize('\u4E05'), '\u4E05');
        Assert.AreEqual(NGram.normalize('\u4E06'), '\u4E06');
        Assert.AreEqual(NGram.normalize('\u4E07'), '\u4E07');
        Assert.AreEqual(NGram.normalize('\u4E08'), '\u4E08');
        Assert.AreEqual(NGram.normalize('\u4E09'), '\u4E09');
        Assert.AreEqual(NGram.normalize('\u4E10'), '\u4E10');
        Assert.AreEqual(NGram.normalize('\u4E11'), '\u4E11');
        Assert.AreEqual(NGram.normalize('\u4E12'), '\u4E12');
        Assert.AreEqual(NGram.normalize('\u4E13'), '\u4E13');
        Assert.AreEqual(NGram.normalize('\u4E14'), '\u4E14');
        Assert.AreEqual(NGram.normalize('\u4E15'), '\u4E15');
        Assert.AreEqual(NGram.normalize('\u4E1e'), '\u4E1e');
        Assert.AreEqual(NGram.normalize('\u4E1f'), '\u4E1f');
        Assert.AreEqual(NGram.normalize('\u4E20'), '\u4E20');
        Assert.AreEqual(NGram.normalize('\u4E21'), '\u4E21');
        Assert.AreEqual(NGram.normalize('\u4E22'), '\u4E22');
        Assert.AreEqual(NGram.normalize('\u4E23'), '\u4E23');
        Assert.AreEqual(NGram.normalize('\u4E24'), '\u4E13');
        Assert.AreEqual(NGram.normalize('\u4E25'), '\u4E13');
        Assert.AreEqual(NGram.normalize('\u4E30'), '\u4E30');
    }

    /**
     * Test method for {@link NGram#normalize(char)} for Romanian characters
     */
    [Test]
    public void testNormalizeForRomanian() {
        Assert.AreEqual(NGram.normalize('\u015f'), '\u015f');
        Assert.AreEqual(NGram.normalize('\u0163'), '\u0163');
        Assert.AreEqual(NGram.normalize('\u0219'), '\u015f');
        Assert.AreEqual(NGram.normalize('\u021b'), '\u0163');
    }

    /**
     * Test method for {@link NGram#get(int)} and {@link NGram#addChar(char)}
     */
    [Test]
    public void testNGram() {
        NGram ngram = new NGram();
        Assert.AreEqual(ngram[0], null);
        Assert.AreEqual(ngram[1], null);
        Assert.AreEqual(ngram[2], null);
        Assert.AreEqual(ngram[3], null);
        Assert.AreEqual(ngram[4], null);
        ngram.AddChar(' ');
        Assert.AreEqual(ngram[1], null);
        Assert.AreEqual(ngram[2], null);
        Assert.AreEqual(ngram[3], null);
        ngram.AddChar('A');
        Assert.AreEqual(ngram[1], "A");
        Assert.AreEqual(ngram[2], " A");
        Assert.AreEqual(ngram[3], null);
        ngram.AddChar('\u06cc');
        Assert.AreEqual(ngram[1], "\u064a");
        Assert.AreEqual(ngram[2], "A\u064a");
        Assert.AreEqual(ngram[3], " A\u064a");
        ngram.AddChar('\u1ea0');
        Assert.AreEqual(ngram[1], "\u1ec3");
        Assert.AreEqual(ngram[2], "\u064a\u1ec3");
        Assert.AreEqual(ngram[3], "A\u064a\u1ec3");
        ngram.AddChar('\u3044');
        Assert.AreEqual(ngram[1], "\u3042");
        Assert.AreEqual(ngram[2], "\u1ec3\u3042");
        Assert.AreEqual(ngram[3], "\u064a\u1ec3\u3042");

        ngram.AddChar('\u30a4');
        Assert.AreEqual(ngram[1], "\u30a2");
        Assert.AreEqual(ngram[2], "\u3042\u30a2");
        Assert.AreEqual(ngram[3], "\u1ec3\u3042\u30a2");
        ngram.AddChar('\u3106');
        Assert.AreEqual(ngram[1], "\u3105");
        Assert.AreEqual(ngram[2], "\u30a2\u3105");
        Assert.AreEqual(ngram[3], "\u3042\u30a2\u3105");
        ngram.AddChar('\uac01');
        Assert.AreEqual(ngram[1], "\uac00");
        Assert.AreEqual(ngram[2], "\u3105\uac00");
        Assert.AreEqual(ngram[3], "\u30a2\u3105\uac00");
        ngram.AddChar('\u2010');
        Assert.AreEqual(ngram[1], null);
        Assert.AreEqual(ngram[2], "\uac00 ");
        Assert.AreEqual(ngram[3], "\u3105\uac00 ");

        ngram.AddChar('a');
        Assert.AreEqual(ngram[1], "a");
        Assert.AreEqual(ngram[2], " a");
        Assert.AreEqual(ngram[3], null);

    }
 
    /**
     * Test method for {@link NGram#get(int)} and {@link NGram#addChar(char)}
     */
    [Test]
    public void testNGram3() {
        NGram ngram = new NGram();

        ngram.AddChar('A');
        Assert.AreEqual(ngram[1], "A");
        Assert.AreEqual(ngram[2], " A");
        Assert.AreEqual(ngram[3], null);

        ngram.AddChar('1');
        Assert.AreEqual(ngram[1], null);
        Assert.AreEqual(ngram[2], "A ");
        Assert.AreEqual(ngram[3], " A ");
        
        ngram.AddChar('B');
        Assert.AreEqual(ngram[1], "B");
        Assert.AreEqual(ngram[2], " B");
        Assert.AreEqual(ngram[3], null);
       
    }
 
    /**
     * Test method for {@link NGram#get(int)} and {@link NGram#addChar(char)}
     */
    [Test]
    public void testNormalizeVietnamese() {
        Assert.AreEqual(NGram.normalize_vi(""), "");
        Assert.AreEqual(NGram.normalize_vi("ABC"), "ABC");
        Assert.AreEqual(NGram.normalize_vi("012"), "012");
        Assert.AreEqual(NGram.normalize_vi("\u00c0"), "\u00c0");

        Assert.AreEqual(NGram.normalize_vi("\u0041\u0300"), "\u00C0");
        Assert.AreEqual(NGram.normalize_vi("\u0045\u0300"), "\u00C8");
        Assert.AreEqual(NGram.normalize_vi("\u0049\u0300"), "\u00CC");
        Assert.AreEqual(NGram.normalize_vi("\u004F\u0300"), "\u00D2");
        Assert.AreEqual(NGram.normalize_vi("\u0055\u0300"), "\u00D9");
        Assert.AreEqual(NGram.normalize_vi("\u0059\u0300"), "\u1EF2");
        Assert.AreEqual(NGram.normalize_vi("\u0061\u0300"), "\u00E0");
        Assert.AreEqual(NGram.normalize_vi("\u0065\u0300"), "\u00E8");
        Assert.AreEqual(NGram.normalize_vi("\u0069\u0300"), "\u00EC");
        Assert.AreEqual(NGram.normalize_vi("\u006F\u0300"), "\u00F2");
        Assert.AreEqual(NGram.normalize_vi("\u0075\u0300"), "\u00F9");
        Assert.AreEqual(NGram.normalize_vi("\u0079\u0300"), "\u1EF3");
        Assert.AreEqual(NGram.normalize_vi("\u00C2\u0300"), "\u1EA6");
        Assert.AreEqual(NGram.normalize_vi("\u00CA\u0300"), "\u1EC0");
        Assert.AreEqual(NGram.normalize_vi("\u00D4\u0300"), "\u1ED2");
        Assert.AreEqual(NGram.normalize_vi("\u00E2\u0300"), "\u1EA7");
        Assert.AreEqual(NGram.normalize_vi("\u00EA\u0300"), "\u1EC1");
        Assert.AreEqual(NGram.normalize_vi("\u00F4\u0300"), "\u1ED3");
        Assert.AreEqual(NGram.normalize_vi("\u0102\u0300"), "\u1EB0");
        Assert.AreEqual(NGram.normalize_vi("\u0103\u0300"), "\u1EB1");
        Assert.AreEqual(NGram.normalize_vi("\u01A0\u0300"), "\u1EDC");
        Assert.AreEqual(NGram.normalize_vi("\u01A1\u0300"), "\u1EDD");
        Assert.AreEqual(NGram.normalize_vi("\u01AF\u0300"), "\u1EEA");
        Assert.AreEqual(NGram.normalize_vi("\u01B0\u0300"), "\u1EEB");

        Assert.AreEqual(NGram.normalize_vi("\u0041\u0301"), "\u00C1");
        Assert.AreEqual(NGram.normalize_vi("\u0045\u0301"), "\u00C9");
        Assert.AreEqual(NGram.normalize_vi("\u0049\u0301"), "\u00CD");
        Assert.AreEqual(NGram.normalize_vi("\u004F\u0301"), "\u00D3");
        Assert.AreEqual(NGram.normalize_vi("\u0055\u0301"), "\u00DA");
        Assert.AreEqual(NGram.normalize_vi("\u0059\u0301"), "\u00DD");
        Assert.AreEqual(NGram.normalize_vi("\u0061\u0301"), "\u00E1");
        Assert.AreEqual(NGram.normalize_vi("\u0065\u0301"), "\u00E9");
        Assert.AreEqual(NGram.normalize_vi("\u0069\u0301"), "\u00ED");
        Assert.AreEqual(NGram.normalize_vi("\u006F\u0301"), "\u00F3");
        Assert.AreEqual(NGram.normalize_vi("\u0075\u0301"), "\u00FA");
        Assert.AreEqual(NGram.normalize_vi("\u0079\u0301"), "\u00FD");
        Assert.AreEqual(NGram.normalize_vi("\u00C2\u0301"), "\u1EA4");
        Assert.AreEqual(NGram.normalize_vi("\u00CA\u0301"), "\u1EBE");
        Assert.AreEqual(NGram.normalize_vi("\u00D4\u0301"), "\u1ED0");
        Assert.AreEqual(NGram.normalize_vi("\u00E2\u0301"), "\u1EA5");
        Assert.AreEqual(NGram.normalize_vi("\u00EA\u0301"), "\u1EBF");
        Assert.AreEqual(NGram.normalize_vi("\u00F4\u0301"), "\u1ED1");
        Assert.AreEqual(NGram.normalize_vi("\u0102\u0301"), "\u1EAE");
        Assert.AreEqual(NGram.normalize_vi("\u0103\u0301"), "\u1EAF");
        Assert.AreEqual(NGram.normalize_vi("\u01A0\u0301"), "\u1EDA");
        Assert.AreEqual(NGram.normalize_vi("\u01A1\u0301"), "\u1EDB");
        Assert.AreEqual(NGram.normalize_vi("\u01AF\u0301"), "\u1EE8");
        Assert.AreEqual(NGram.normalize_vi("\u01B0\u0301"), "\u1EE9");

        Assert.AreEqual(NGram.normalize_vi("\u0041\u0303"), "\u00C3");
        Assert.AreEqual(NGram.normalize_vi("\u0045\u0303"), "\u1EBC");
        Assert.AreEqual(NGram.normalize_vi("\u0049\u0303"), "\u0128");
        Assert.AreEqual(NGram.normalize_vi("\u004F\u0303"), "\u00D5");
        Assert.AreEqual(NGram.normalize_vi("\u0055\u0303"), "\u0168");
        Assert.AreEqual(NGram.normalize_vi("\u0059\u0303"), "\u1EF8");
        Assert.AreEqual(NGram.normalize_vi("\u0061\u0303"), "\u00E3");
        Assert.AreEqual(NGram.normalize_vi("\u0065\u0303"), "\u1EBD");
        Assert.AreEqual(NGram.normalize_vi("\u0069\u0303"), "\u0129");
        Assert.AreEqual(NGram.normalize_vi("\u006F\u0303"), "\u00F5");
        Assert.AreEqual(NGram.normalize_vi("\u0075\u0303"), "\u0169");
        Assert.AreEqual(NGram.normalize_vi("\u0079\u0303"), "\u1EF9");
        Assert.AreEqual(NGram.normalize_vi("\u00C2\u0303"), "\u1EAA");
        Assert.AreEqual(NGram.normalize_vi("\u00CA\u0303"), "\u1EC4");
        Assert.AreEqual(NGram.normalize_vi("\u00D4\u0303"), "\u1ED6");
        Assert.AreEqual(NGram.normalize_vi("\u00E2\u0303"), "\u1EAB");
        Assert.AreEqual(NGram.normalize_vi("\u00EA\u0303"), "\u1EC5");
        Assert.AreEqual(NGram.normalize_vi("\u00F4\u0303"), "\u1ED7");
        Assert.AreEqual(NGram.normalize_vi("\u0102\u0303"), "\u1EB4");
        Assert.AreEqual(NGram.normalize_vi("\u0103\u0303"), "\u1EB5");
        Assert.AreEqual(NGram.normalize_vi("\u01A0\u0303"), "\u1EE0");
        Assert.AreEqual(NGram.normalize_vi("\u01A1\u0303"), "\u1EE1");
        Assert.AreEqual(NGram.normalize_vi("\u01AF\u0303"), "\u1EEE");
        Assert.AreEqual(NGram.normalize_vi("\u01B0\u0303"), "\u1EEF");

        Assert.AreEqual(NGram.normalize_vi("\u0041\u0309"), "\u1EA2");
        Assert.AreEqual(NGram.normalize_vi("\u0045\u0309"), "\u1EBA");
        Assert.AreEqual(NGram.normalize_vi("\u0049\u0309"), "\u1EC8");
        Assert.AreEqual(NGram.normalize_vi("\u004F\u0309"), "\u1ECE");
        Assert.AreEqual(NGram.normalize_vi("\u0055\u0309"), "\u1EE6");
        Assert.AreEqual(NGram.normalize_vi("\u0059\u0309"), "\u1EF6");
        Assert.AreEqual(NGram.normalize_vi("\u0061\u0309"), "\u1EA3");
        Assert.AreEqual(NGram.normalize_vi("\u0065\u0309"), "\u1EBB");
        Assert.AreEqual(NGram.normalize_vi("\u0069\u0309"), "\u1EC9");
        Assert.AreEqual(NGram.normalize_vi("\u006F\u0309"), "\u1ECF");
        Assert.AreEqual(NGram.normalize_vi("\u0075\u0309"), "\u1EE7");
        Assert.AreEqual(NGram.normalize_vi("\u0079\u0309"), "\u1EF7");
        Assert.AreEqual(NGram.normalize_vi("\u00C2\u0309"), "\u1EA8");
        Assert.AreEqual(NGram.normalize_vi("\u00CA\u0309"), "\u1EC2");
        Assert.AreEqual(NGram.normalize_vi("\u00D4\u0309"), "\u1ED4");
        Assert.AreEqual(NGram.normalize_vi("\u00E2\u0309"), "\u1EA9");
        Assert.AreEqual(NGram.normalize_vi("\u00EA\u0309"), "\u1EC3");
        Assert.AreEqual(NGram.normalize_vi("\u00F4\u0309"), "\u1ED5");
        Assert.AreEqual(NGram.normalize_vi("\u0102\u0309"), "\u1EB2");
        Assert.AreEqual(NGram.normalize_vi("\u0103\u0309"), "\u1EB3");
        Assert.AreEqual(NGram.normalize_vi("\u01A0\u0309"), "\u1EDE");
        Assert.AreEqual(NGram.normalize_vi("\u01A1\u0309"), "\u1EDF");
        Assert.AreEqual(NGram.normalize_vi("\u01AF\u0309"), "\u1EEC");
        Assert.AreEqual(NGram.normalize_vi("\u01B0\u0309"), "\u1EED");

        Assert.AreEqual(NGram.normalize_vi("\u0041\u0323"), "\u1EA0");
        Assert.AreEqual(NGram.normalize_vi("\u0045\u0323"), "\u1EB8");
        Assert.AreEqual(NGram.normalize_vi("\u0049\u0323"), "\u1ECA");
        Assert.AreEqual(NGram.normalize_vi("\u004F\u0323"), "\u1ECC");
        Assert.AreEqual(NGram.normalize_vi("\u0055\u0323"), "\u1EE4");
        Assert.AreEqual(NGram.normalize_vi("\u0059\u0323"), "\u1EF4");
        Assert.AreEqual(NGram.normalize_vi("\u0061\u0323"), "\u1EA1");
        Assert.AreEqual(NGram.normalize_vi("\u0065\u0323"), "\u1EB9");
        Assert.AreEqual(NGram.normalize_vi("\u0069\u0323"), "\u1ECB");
        Assert.AreEqual(NGram.normalize_vi("\u006F\u0323"), "\u1ECD");
        Assert.AreEqual(NGram.normalize_vi("\u0075\u0323"), "\u1EE5");
        Assert.AreEqual(NGram.normalize_vi("\u0079\u0323"), "\u1EF5");
        Assert.AreEqual(NGram.normalize_vi("\u00C2\u0323"), "\u1EAC");
        Assert.AreEqual(NGram.normalize_vi("\u00CA\u0323"), "\u1EC6");
        Assert.AreEqual(NGram.normalize_vi("\u00D4\u0323"), "\u1ED8");
        Assert.AreEqual(NGram.normalize_vi("\u00E2\u0323"), "\u1EAD");
        Assert.AreEqual(NGram.normalize_vi("\u00EA\u0323"), "\u1EC7");
        Assert.AreEqual(NGram.normalize_vi("\u00F4\u0323"), "\u1ED9");
        Assert.AreEqual(NGram.normalize_vi("\u0102\u0323"), "\u1EB6");
        Assert.AreEqual(NGram.normalize_vi("\u0103\u0323"), "\u1EB7");
        Assert.AreEqual(NGram.normalize_vi("\u01A0\u0323"), "\u1EE2");
        Assert.AreEqual(NGram.normalize_vi("\u01A1\u0323"), "\u1EE3");
        Assert.AreEqual(NGram.normalize_vi("\u01AF\u0323"), "\u1EF0");
        Assert.AreEqual(NGram.normalize_vi("\u01B0\u0323"), "\u1EF1");

    }
}