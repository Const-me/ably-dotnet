using Ably.MessageEncoders;
using Ably.Rest;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Ably.Tests.MessageEncodes
{
    public class JsonEncoderTests
    {
        private object _objectData;
        private string _jsonData;
        private int[] _arrayData = new []{ 1, 2, 3};
        private string _jsonArrayData = "[1,2,3]";
        private JsonEncoder encoder;

        public JsonEncoderTests()
        {
            _objectData = new { Test = "test", Best = "best"};
            _jsonData = JsonConvert.SerializeObject(_objectData);
            encoder = new JsonEncoder(Protocol.MsgPack);
        }

        private Message EncodePayload(object data, string encoding = "")
        {
            var payload = new Message() {Data = data, Encoding = encoding};
            encoder.Encode(payload, new ChannelOptions());
            return payload;
        }

        private Message DecodePayload(object data, string encoding = "")
        {
            var payload = new Message() { Data = data, Encoding = encoding };
            encoder.Decode(payload, new ChannelOptions());
            return payload;
        }

        public class Decode : JsonEncoderTests
        {
            [Fact]
            public void WithJsonPayload_ConvertsDataToJObjectAndStripsEncoding()
            {
                var payload = DecodePayload(_jsonData, "json");
                
                payload.Data.Should().BeOfType<JObject>();

                var obj =(payload.Data as JObject).ToObject(_objectData.GetType());
                obj.Should().Be(_objectData);

                payload.Encoding.Should().BeEmpty();
            }

            [Fact]
            public void WithJsonPayloadBeforeOtherPayload_ConvertsDataToJObjecAndStrinpsEncoding()
            {
                var payload = DecodePayload(_jsonData, "utf-8/json");

                payload.Data.Should().BeOfType<JObject>();
                payload.Encoding.Should().Be("utf-8");
            }

            [Fact]
            public void WithAnotherPayload_LeavesDataAndEncoding()
            {
                var payload = DecodePayload("test", "utf-8");

                payload.Data.Should().Be("test");
                payload.Encoding.Should().Be("utf-8");
            }

            [Fact]
            public void WithInvalidJsonPayload_ThrowsAblyException()
            {
                var error = Assert.Throws<AblyException>(delegate { DecodePayload("test", "json"); });
                error.ErrorInfo.Reason.Should().Be("Invalid Json data: 'test'");
            }
           

        }

        public class Encode : JsonEncoderTests
        {
            [Fact]
            public void WithObject_ConvertDataToJsonStringAndSetsCorrectEncoding()
            {
                var payload = EncodePayload(_objectData);

                payload.Data.Should().Be(_jsonData);
                payload.Encoding.Should().Be("json");
            }

            [Fact]
            public void WithObjectWithExistingEncoding_ConvertsDataAndAppendsEncoding()
            {
                var payload = EncodePayload(_objectData, "utf-8");

                payload.Data.Should().Be(_jsonData);
                payload.Encoding.Should().Be("utf-8/json");
            }

            [Fact]
            public void WithArray_ConvertsDataCorrectly()
            {
                var payload = EncodePayload(_arrayData);

                payload.Data.Should().Be(_jsonArrayData);
                payload.Encoding.Should().Be("json");
            }

            [Fact]
            public void WithStringData_LeavesEncodingAndDataIntact()
            {
                var payload = EncodePayload("test");

                payload.Data.Should().Be("test");
                payload.Encoding.Should().BeEmpty();
            }

            [Fact]
            public void WithNullData_LeavesEncodingAndDataIntact()
            {
                var payload = EncodePayload(null);

                payload.Data.Should().BeNull();
                payload.Encoding.Should().BeEmpty();
            }
        }

    }
}