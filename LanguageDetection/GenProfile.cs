using LanguageDetection.Utils;
using System;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace LanguageDetection
{
    /**
    * Load Wikipedia's abstract XML as corpus and
    * generate its language profile in JSON format.
    * 
    * @author Nakatani Shuyo
    * 
    */
    public class GenProfile
    {

        /**
         * Load Wikipedia abstract database file and generate its language profile
         * @param lang target language name
         * @param file target database file path
         * @return Language profile instance
         * @throws LangDetectException 
         */
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


        /**
         * Load text file with UTF-8 and generate its language profile
         * @param lang target language name
         * @param file target file path
         * @return Language profile instance
         * @throws LangDetectException 
         */
        public static LangProfile LoadFromText(string lang, string file)
        {

            LangProfile profile = new LangProfile(lang);

            StreamReader strm = null;
            try
            {
                strm = new StreamReader(File.OpenRead(file));

                int count = 0;
                while (!strm.EndOfStream)
                {
                    string line = strm.ReadLine();
                    profile.Update(line);
                    ++count;
                }

                Console.WriteLine(lang + ":" + count);

            }
            catch (IOException e)
            {
                throw new LangDetectException(ErrorCode.CantOpenTrainData, "Can't open training database file '" + file + "'");
            }
            finally
            {
                try
                {
                    if (strm != null) strm.Close();
                }
                catch (IOException e) { }
            }
            return profile;
        }
    }
}