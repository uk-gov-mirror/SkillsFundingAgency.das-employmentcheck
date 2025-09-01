using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace SFA.DAS.EmploymentCheck.Functions.UnitTests.TestHelpers
{
    internal sealed class TestHttpRequestData : HttpRequestData
    {
        private readonly MemoryStream _body = new MemoryStream();
        private readonly IReadOnlyCollection<IHttpCookie> _cookies;
        private readonly IReadOnlyCollection<ClaimsIdentity> _identities;

        public TestHttpRequestData(FunctionContext functionContext, string method = "GET", Uri url = null)
            : base(functionContext)
        {
            Method = method;
            Url = url ?? new Uri("http://localhost/");
            Headers = new HttpHeadersCollection();
            _cookies = Array.Empty<IHttpCookie>();
            _identities = Array.Empty<ClaimsIdentity>();
        }

        public override Stream Body => _body;
        public override HttpHeadersCollection Headers { get; }
        public override IReadOnlyCollection<IHttpCookie> Cookies => _cookies;
        public override IReadOnlyCollection<ClaimsIdentity> Identities => _identities;
        public override Uri Url { get; }
        public override string Method { get; }

        public override HttpResponseData CreateResponse() => new TestHttpResponseData(FunctionContext);
    }

    internal sealed class TestHttpResponseData : HttpResponseData
    {
        public TestHttpResponseData(FunctionContext functionContext)
            : base(functionContext)
        {
            Body = new MemoryStream();
            Headers = new HttpHeadersCollection();
            Cookies = new TestHttpCookies();
        }

        public override HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public override HttpHeadersCollection Headers { get; set; }
        public override Stream Body { get; set; }
        public override HttpCookies Cookies { get; }
        public string ReadBodyAsString()
        {
            Body.Position = 0;
            using var sr = new StreamReader(Body, Encoding.UTF8, false, 1024, leaveOpen: true);
            return sr.ReadToEnd();
        }
    }

    internal sealed class TestHttpCookies : HttpCookies
    {
        public override IHttpCookie CreateNew() => new TestHttpCookie();
        public override void Append(string name, string value) { }
        public override void Append(IHttpCookie cookie) { }
    }

    internal sealed class TestHttpCookie : IHttpCookie
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Domain { get; set; }
        public string Path { get; set; }
        public DateTimeOffset? Expires { get; set; }
        public SameSite SameSite { get; set; }
        public bool? Secure { get; set; }
        public bool? HttpOnly { get; set; }
        public double? MaxAge { get; set; }
    }
}
