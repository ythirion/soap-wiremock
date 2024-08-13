using System.ServiceModel;
using FluentAssertions;
using NUnit.Framework;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.SoapDemo
{
    [TestFixture]
    public class DemoTests
    {
        private const string ServiceUrl = "http://www.dataaccess.com/webservicesserver/";

        private WireMockServer _server;
        private NumberConversionSoapType _client;

        [SetUp]
        public void SetUp()
        {
            // Setup the WireMock server and the client
            _server = WireMockServer.Start(new WireMockServerSettings
            {
                Logger = new WireMockConsoleLogger(),
                Urls = new[] {"http://localhost:20001/"},
                StartAdminInterface = true,
                ReadStaticMappings = true,
                WatchStaticMappings = true,
                WatchStaticMappingsInSubdirectories = true,
                AllowPartialMapping = true
            });

            _client = new NumberConversionSoapTypeClient(
                "NumberConversionSoapTypeClient",
                new EndpointAddress(_server.Urls[0])
            );
        }

        [TearDown]
        public void TearDown() => _server.Stop();

        [Test]
        public void Calculator()
        {
            const string action = "NumberToDollars";

            var soapRequestBody =
                new SoapMessageBuilder(ServiceUrl, action)
                    .AddParameter("num:dNum", "12")
                    .BuildRequest();

            var soapResponseBody =
                new SoapMessageBuilder(ServiceUrl, action)
                    .BuildResponse("NumberToDollarsResult", "42");

            // Setup WireMock
            // Simulate the SOAP response for a given parameter
            _server.Given(
                Request.Create()
                    .WithHeader("SOAPAction", "http://www.dataaccess.com/webservicesserver/NumberToDollars")
                    .UsingPost()
                    .WithBody(soapRequestBody)
            ).RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "text/xml; charset=utf-8")
                    .WithBody(soapResponseBody)
            );

            // Simulate static code that calls SOAP services
            Converter.ToDollars(_client, 12)
                .Should()
                .Be(42);
        }
    }
}