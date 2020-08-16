using System;

namespace LanguageDetection
{
    /// <summary>
    /// </summary>
    public enum ErrorCode
    {
        NoTextError, FormatError, FileLoadError, DuplicateLangError, NeedLoadProfileError, CantDetectError, CantOpenTrainData, TrainDataFormatError, InitParamError
    }

    /// <summary>
    /// </summary>
    public class LangDetectException : Exception
    {
        /// <summary>
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public LangDetectException(ErrorCode code, string message) : base(message)
        {
            this.Code = code;
        }

        /// <summary>
        /// </summary>
        /// <returns>the error code</returns>
        public ErrorCode Code { get; }
    }
}
