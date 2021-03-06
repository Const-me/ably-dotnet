using System;
using System.Diagnostics;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Ably
{
    /// <summary>
    /// An exception type encapsulating error informaiton containing an Ably specific error code and generic status code
    /// </summary>
    public class ErrorInfo
    {
        internal static readonly ErrorInfo ReasonClosed = new ErrorInfo("Connection closed by client", 10000);
        internal static readonly ErrorInfo ReasonDisconnected = new ErrorInfo("Connection temporarily unavailable", 80003);
        internal static readonly ErrorInfo ReasonSuspended = new ErrorInfo("Connection unavailable", 80002);
        internal static readonly ErrorInfo ReasonFailed = new ErrorInfo("Connection failed", 80000);
        internal static readonly ErrorInfo ReasonRefused = new ErrorInfo("Access refused", 40100);
        internal static readonly ErrorInfo ReasonTooBig = new ErrorInfo("Connection closed; message too large", 40000);
        internal static readonly ErrorInfo ReasonNeverConnected = new ErrorInfo("Unable to establish connection", 80002);
        internal static readonly ErrorInfo ReasonTimeout = new ErrorInfo("Unable to establish connection", 80014);
        internal static readonly ErrorInfo ReasonUnknown = new ErrorInfo("Unknown error", 50000, System.Net.HttpStatusCode.InternalServerError);

        internal const string CodePropertyName = "code";
        internal const string StatusCodePropertyName = "statusCode";
        internal const string ReasonPropertyName = "message";

        /// <summary>
        /// Ably error code (see https://github.com/ably/ably-common/blob/master/protocol/errors.json)
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// The http status code corresponding to this error
        /// </summary>
        public HttpStatusCode? StatusCode { get; set; }
        /// <summary>
        /// Additional reason information, where available
        /// </summary>
        public string Reason { get; set; }

        public ErrorInfo(string reason, int code)
        {
            Code = code;
            Reason = reason;
        }

        public ErrorInfo(string reason, int code, HttpStatusCode? statusCode = null)
        {
            Code = code;
            StatusCode = statusCode;
            Reason = reason;
        }

        public override string ToString()
        {
            if (StatusCode.HasValue == false)
            {
                return string.Format("Reason: {0}; Code: {1}", Reason, Code);
            }
            return string.Format("Reason: {0}; Code: {1}; HttpStatusCode: ({2}){3}", Reason, Code, (int)StatusCode.Value, StatusCode);
        }

        internal static ErrorInfo Parse(AblyResponse response)
        {
            string reason = "";
            int errorCode = 500;

            if (response.Type == ResponseType.Json)
            {

                try
                {
                    var json = JObject.Parse(response.TextResponse);
                    if (json["error"] != null)
                    {
                        reason = (string)json["error"]["message"];
                        errorCode = (int)json["error"]["code"];
                    }
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.Message);
                    //If there is no json or there is something wrong we don't want to throw from here. The
                }
            }
            return new ErrorInfo(reason.IsEmpty() ? "Unknown error" : reason, errorCode, response.StatusCode);
        }
    }
}