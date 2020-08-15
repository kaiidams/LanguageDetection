


using System;
/**
* @author Nakatani Shuyo
*/
public enum ErrorCode {
    NoTextError, FormatError, FileLoadError, DuplicateLangError, NeedLoadProfileError, CantDetectError, CantOpenTrainData, TrainDataFormatError, InitParamError
}

/**
 * @author Nakatani Shuyo
 *
 */
public class LangDetectException : Exception {
    private const long serialVersionUID = 1L;
    private ErrorCode code;
    

    /**
     * @param code
     * @param message
     */
    public LangDetectException(ErrorCode code, string message) : base(message) {
        this.code = code;
    }

    /**
     * @return the error code
     */
    public ErrorCode getCode() {
        return code;
    }
}
