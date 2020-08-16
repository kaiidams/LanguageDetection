using LanguageDetection.Utils;
using System;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace LanguageDetection
{
    /// <summary>
    /// Load Wikipedia's abstract XML as corpus and
    /// generate its language profile in JSON format.
    /// </summary>
    public class GenProfile
    {

        /// <summary>
        /// Load Wikipedia abstract database file and generate its language profile
        /// </summary>
        /// <param name="lang">target language name</param>
        /// <param name="file">target database file path</param>
        /// <returns>Language profile instance</returns>
        /// <exception>LangDetectException </exception>
        public static LangProfile LoadFromWikipediaAbstract(string lang, string file)
        {

            LangProfile profile = new LangProfile(lang);

            StreamReader br = null;
            try
            {
                Stream strm = File.OpenRead(file);
                if (file.EndsWith(".gz")) strm = new GZipStream(strm, CompressionMode.Decompress);
                br = new StreamReader(strm);

                TagExtractor tagextractor = new TagExtractor("abstract", 100);

                XmlReader reader = XmlReader.Create(br);
                try
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                tagextractor.SetTag(reader.Name);
                                break;
                            case XmlNodeType.Text:
                                tagextractor.Add(reader.Value);
                                break;
                            case XmlNodeType.EndElement:
                                string text = tagextractor.CloseTag();
                                if (text != null) profile.Update(text);
                                break;
                        }
                    }
                }
                catch (XmlException e)
                {
                    throw new LangDetectException(ErrorCode.TrainDataFormatError, "Training database file '" + file + "' is an invalid XML.");
                }
                finally
                {
                    try
                    {
                        if (reader != null) reader.Close();
                    }
                    catch (XmlException e) { }
                }
                Console.WriteLine(lang + ":" + tagextractor.Count());

            }
            catch (IOException e)
            {
                throw new LangDetectException(ErrorCode.CantOpenTrainData, "Can't open training database file '" + file + "'");
            }
            finally
            {
                try
                {
                    if (br != null) br.Close();
                }
                catch (IOException e) { }
            }
            return profile;
        }


        /// <summary>
        /// Load text file with UTF-8 and generate its language profile
        /// </summary>
        /// <param name="lang">target language name</param>
        /// <param name="file">target file path</param>
        /// <returns>Language profile instance</returns>
        /// <exception>LangDetectException </exception>

        public static LangProfile LoadFromText(string lang, string file)
        {

            LangProfile profile = new LangProfile(lang);

            try
            {
                using (var strm = new StreamReader(File.OpenRead(file)))
                {
                    int count = 0;
                    string line;
                    while ((line = strm.ReadLine()) != null)
                    {
                        profile.Update(line);
                        ++count;
                    }

                    Console.WriteLine(lang + ":" + count);
                }
            }
            catch (IOException)
            {
                throw new LangDetectException(ErrorCode.CantOpenTrainData, "Can't open training database file '" + file + "'");
            }

            return profile;
        }
    }
}
