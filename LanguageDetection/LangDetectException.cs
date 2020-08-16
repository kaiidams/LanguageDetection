using System;

namespace LanguageDetection
{
    /**
    * @author Nakatani Shuyo
*/
    public enum ErrorCode
    {
        NoTextError, FormatError, FileLoadError, DuplicateLangError, NeedLoadProfileError, CantDetectError, CantOpenTrainData, TrainDataFormatError, InitParamError
    }

    /**
     * @author Nakatani Shuyo
     *
     */
    public class LangDetectException : Exception
    {
        /**
         * @param code
         * @param message
         */
        public LangDetectException(ErrorCode code, string message) : base(message)
        {
            this.Code = code;
        }

        /**
         * @return the error code
         */
        public ErrorCode Code { get; }
    }
}