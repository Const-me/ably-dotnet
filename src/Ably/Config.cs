using System;
using Ably.Encryption;

namespace Ably
{
    public static class Config
    {
        /// <summary>
        /// Http connection timeout in ms.
        /// Default value: 15000 ms
        /// </summary>
        public static Func<CipherParams, IChannelCipher> GetCipher = @params => new AesCipher(@params);
        internal static string DefaultHost = "rest.ably.io";

        /// <summary>The default log level you'll see in the debug output.</summary>
        internal const LogLevel DefaultLogLevel = LogLevel.Info;

        /// <summary>X-Ably-Version HTTP request header value</summary>
        internal const string AblyVersion = "0.8";

        internal static Func<DateTimeOffset> Now = () => DateTimeOffset.UtcNow;

        public static string Host = "rest.ably.io";
        public const int Port = 80;
        public const int TlsPort = 443;
        public const int ConnectTimeout = 15000;
        public const int CommulativeFailedRequestTimeOutInSeconds = 10;
        public const int DisconnectTimeout = 10000;
        public const int SuspendedTimeout = 60000;
        public static string[] Transports = { "web_socket", "comet" };

        public static string[] FallbackHosts = { "A.ably-realtime.com", "B.ably-realtime.com", "C.ably-realtime.com", "D.ably-realtime.com", "E.ably-realtime.com" };

        public const int Limit = 100;
        public static int ProtocolVersion = 1;
    }
}