using static System.Decimal;

namespace WireMock.SoapDemo
{
    public static class Converter
    {
        public static decimal ToDollars(NumberConversionSoapType client, decimal number) => Parse(client.NumberToDollars(number));
    }
}