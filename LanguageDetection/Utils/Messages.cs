


using System.Resources;
/**
* This is {@link Messages} class generated by Eclipse automatically.
* Users don't use this class directly.
* @author Nakatani Shuyo
*/
public static class Messages {
    static readonly ResourceManager rm;

    static Messages() {
        rm = new ResourceManager("Messages",
                               typeof(Messages).Assembly);
    }

    public static string GetString(string key) {
        return rm.GetString(key);
    }
}
