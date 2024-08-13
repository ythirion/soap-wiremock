using System.Collections.Generic;
using System.Linq;

namespace WireMock.SoapDemo
{
    public class SoapMessageBuilder
    {
        private readonly string _namespace;
        private readonly string _action;
        private readonly Dictionary<string, string> _parameters;

        public SoapMessageBuilder(string ns, string action)
        {
            _namespace = ns;
            _action = action;
            _parameters = new Dictionary<string, string>();
        }

        public SoapMessageBuilder AddParameter(string name, string value)
        {
            _parameters[name] = value;
            return this;
        }

        public string BuildRequest() 
            => $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:num=""{_namespace}"">
                   <soapenv:Header/>
                   <soapenv:Body>
                      <num:{_action}>
                         {string.Join("", _parameters.Select(p => $"<{p.Key}>{p.Value}</{p.Key}>"))}
                      </num:{_action}>
                   </soapenv:Body>
                </soapenv:Envelope>";

        public string BuildResponse(string responseElement, string responseValue)
            => $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"">
                   <soapenv:Body>
                      <{_action}Response xmlns=""{_namespace}"">
                         <{responseElement}>{responseValue}</{responseElement}>
                      </{_action}Response>
                   </soapenv:Body>
                </soapenv:Envelope>";
    }
}