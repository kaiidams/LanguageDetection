using LanguageDetection;
using LanguageDetection.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LanguageDetectionTest
{
    /// <summary>
    /// Unit test for {@link Detector} and {@link DetectorFactory}.
    /// </summary>
    public class DetectorTest
    {

        private const string TRAINING_EN = "a a a b b c c d e";
        private const string TRAINING_FR = "a b b c c c d d d";
        private const string TRAINING_JA = "\u3042 \u3042 \u3042 \u3044 \u3046 \u3048 \u3048";
        private const string JSON_LANG1 = "{\"freq\":{\"A\":3,\"B\":6,\"C\":3,\"AB\":2,\"BC\":1,\"ABC\":2,\"BBC\":1,\"CBA\":1},\"n_words\":[12,3,4],\"name\":\"lang1\"}";
        private const string JSON_LANG2 = "{\"freq\":{\"A\":6,\"B\":3,\"C\":3,\"AA\":3,\"AB\":2,\"ABC\":1,\"ABA\":1,\"CAA\":1},\"n_words\":[12,5,3],\"name\":\"lang2\"}";

        [SetUp]
        public void setUp()
        {
            DetectorFactory.Clear();

            LangProfile profile_en = new LangProfile("en");
            foreach (string w in TRAINING_EN.Split(" "))
                profile_en.Add(w);
            DetectorFactory.AddProfile(profile_en, 0, 3);

            LangProfile profile_fr = new LangProfile("fr");
            foreach (string w in TRAINING_FR.Split(" "))
                profile_fr.Add(w);
            DetectorFactory.AddProfile(profile_fr, 1, 3);

            LangProfile profile_ja = new LangProfile("ja");
            foreach (string w in TRAINING_JA.Split(" "))
                profile_ja.Add(w);
            DetectorFactory.AddProfile(profile_ja, 2, 3);
        }

        [TearDown]
        public void tearDown()
        {
        }

        [Test]
        public void testDetector1()
        {
            Detector detect = DetectorFactory.Create();
            detect.Append("a");
            Assert.AreEqual(detect.Detect(), "en");
        }

        [Test]
        public void testDetector2()
        {
            Detector detect = DetectorFactory.Create();
            detect.Append("b d");
            Assert.AreEqual(detect.Detect(), "fr");
        }

        [Test]
        public void testDetector3()
        {
            Detector detect = DetectorFactory.Create();
            detect.Append("d e");
            Assert.AreEqual(detect.Detect(), "en");
        }

        [Test]
        public void testDetector4()
        {
            Detector detect = DetectorFactory.Create();
            detect.Append("\u3042\u3042\u3042\u3042a");
            Assert.AreEqual(detect.Detect(), "ja");
        }

        [Test]
        public void testLangList()
        {
            IList<string> langList = DetectorFactory.GetLangList();
            Assert.AreEqual(langList.Count, 3);
            Assert.AreEqual(langList[0], "en");
            Assert.AreEqual(langList[1], "fr");
            Assert.AreEqual(langList[2], "ja");
        }

        [Test]
        public void testLangListException()
        {
            IList<string> langList = DetectorFactory.GetLangList();
            Assert.Throws<NotSupportedException>(() => langList.Add("hoge"));
            //langList.Add(1, "hoge");
        }

        [Test]
        public void testFactoryFromJsonString()
        {
            DetectorFactory.Clear();
            List<string> profiles = new List<string>();
            profiles.Add(JSON_LANG1);
            profiles.Add(JSON_LANG2);
            DetectorFactory.LoadProfile(profiles);
            IList<string> langList = DetectorFactory.GetLangList();
            Assert.AreEqual(langList.Count, 2);
            Assert.AreEqual(langList[0], "lang1");
            Assert.AreEqual(langList[1], "lang2");
        }
    }
}
