using System.Text;

namespace LanguageDetection.Utils
{
    /// <summary>
    /// {@link TagExtractor} is a class which extracts inner texts of specified tag.
    /// Users don't use this class directly.
    /// </summary>
    public class TagExtractor
    {
        public /* package scope */ string target_;        
        public /* package scope */ int threshold_;
        public /* package scope */ StringBuilder buf_;
        public /* package scope */ string tag_;
        private int count_;

        public TagExtractor(string tag, int threshold)
        {
            target_ = tag;
            threshold_ = threshold;
            count_ = 0;
            Clear();
        }
        public int Count()
        {
            return count_;
        }
        public void Clear()
        {
            buf_ = new StringBuilder();
            tag_ = null;
        }
        public void SetTag(string tag)
        {
            tag_ = tag;
        }
        public void Add(string line)
        {
            if (tag_ == target_ && line != null)
            {
                buf_.Append(line);
            }
        }
        public string CloseTag()
        {
            string st = null;
            if (tag_ == target_ && buf_.Length > threshold_)
            {
                st = buf_.ToString();
                ++count_;
            }
            Clear();
            return st;
        }
    }
}


