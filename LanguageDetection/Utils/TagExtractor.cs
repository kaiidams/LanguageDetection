using System.Text;

namespace LanguageDetection.Utils
{
    /**
    * {@link TagExtractor} is a class which extracts inner texts of specified tag.
    * Users don't use this class directly.
    * @author Nakatani Shuyo
    */
    public class TagExtractor {
        /* package scope */ public string target_;
        /* package scope */ public int threshold_;
        /* package scope */ public StringBuilder buf_;
        /* package scope */ public string tag_;
        private int count_;

        public TagExtractor(string tag, int threshold) {
            target_ = tag;
            threshold_ = threshold;
            count_ = 0;
            Clear();
        }
        public int Count() {
            return count_;
        }
        public void Clear() {
            buf_ = new StringBuilder();
            tag_ = null;
        }
        public void SetTag(string tag){
            tag_ = tag;
        }
        public void Add(string line) {
            if (tag_ == target_ && line != null) {
                buf_.Append(line);
            }
        }
        public string CloseTag() {
            string st = null;
            if (tag_ == target_ && buf_.Length > threshold_) {
                st = buf_.ToString();
                ++count_;
            }
            Clear();
            return st;
        }
    }
}

