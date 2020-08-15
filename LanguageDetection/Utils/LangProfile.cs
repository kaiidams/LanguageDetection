


using System.Collections.Generic;
/**
* {@link LangProfile} is a Language Profile Class.
* Users don't use this class directly.
* 
* @author Nakatani Shuyo
*/
public class LangProfile {
    private const int MINIMUM_FREQ = 2;
    private const int LESS_FREQ_RATIO = 100000;
    public string name = null;
    public Dictionary<string, int> freq = new Dictionary<string, int>();
    public int[] n_words = new int[NGram.N_GRAM];

    /**
     * Constructor for JSONIC 
     */
    public LangProfile() {}

    /**
     * Normal Constructor
     * @param name language name
     */
    public LangProfile(string name) {
        this.name = name;
    }
    
    /**
     * Add n-gram to profile
     * @param gram
     */
    public void Add(string gram) {
        if (name == null || gram == null) return;   // Illegal
        int len = gram.Length;
        if (len < 1 || len > NGram.N_GRAM) return;  // Illegal
        ++n_words[len - 1];
        if (freq.ContainsKey(gram)) {
            freq[gram] = freq[gram] + 1;
        } else {
            freq[gram] = 1;
        }
    }

    /**
     * Eliminate below less frequency n-grams and noise Latin alphabets
     */
    public void OmitLessFreq() {
        if (name == null) return;   // Illegal
        int threshold = n_words[0] / LESS_FREQ_RATIO;
        if (threshold < MINIMUM_FREQ) threshold = MINIMUM_FREQ;
        
        var keys = freq.Keys;
        int roman = 0;
        for(Iterator<string> i = keys.iterator(); i.hasNext(); ){
            string key = i.next();
            int count = freq[key];
            if (count <= threshold) {
                n_words[key.Length-1] -= count; 
                i.remove();
            } else {
                if (key.matches("^[A-Za-z]$")) {
                    roman += count;
                }
            }
        }

        // roman check
        if (roman < n_words[0] / 3) {
            HashSet<string> keys2 = freq.Keys;
            for(Iterator<string> i = keys2.iterator(); i.hasNext(); ){
                string key = i.next();
                if (key.matches(".*[A-Za-z].*")) {
                    n_words[key.Length-1] -= freq[key]; 
                    i.remove();
                }
            }
            
        }
    }

    /**
     * Update the language profile with (fragmented) text.
     * Extract n-grams from text and add their frequency into the profile.
     * @param text (fragmented) text to extract n-grams
     */
    public void Update(string text) {
        if (text == null) return;
        text = NGram.normalize_vi(text);
        NGram gram = new NGram();
        for(int i=0; i<text.Length; ++i) {
            gram.AddChar(text[i]);
            for(int n=1; n<=NGram.N_GRAM; ++n) {
                Add(gram[n]);
            }
        }
    }
}
