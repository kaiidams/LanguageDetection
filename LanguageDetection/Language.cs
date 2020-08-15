/**
 * {@link Language} is to store the detected language.
 * {@link Detector#getProbabilities()} returns an {@link List} of {@link Language}s.
 *  
 * @see Detector#getProbabilities()
 * @author Nakatani Shuyo
 *
 */
public class Language {
    public string lang;
    public double prob;
    public Language(string lang, double prob) {
        this.lang = lang;
        this.prob = prob;
    }
    public override string ToString() {
        if (lang==null) return "";
        return lang + ":" + prob;
    }
}
