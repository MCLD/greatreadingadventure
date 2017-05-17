using Microsoft.AspNetCore.Http;

namespace GRA.CommandLine.FakeWeb
{
    class FakeHttpContext : IHttpContextAccessor
    {
        private HttpContext _httpContext;
        public FakeHttpContext()
        {
            _httpContext = new DefaultHttpContext()
            {
                Session = new FakeSession()
            };
        }

        public HttpContext HttpContext
        {
            get
            {
                return _httpContext;
            }
            set
            {
                _httpContext = value;
            }
        }
    }
}
