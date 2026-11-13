namespace RaceDay.Mvc.Services
{
    public class CookieForwardingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieForwardingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;
            var cookieDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // 1. Load existing cookies from MVC Session
            var savedCookies = context?.Session.GetString("ApiCookies");
            if (!string.IsNullOrEmpty(savedCookies))
            {
                foreach (var item in savedCookies.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = item.Trim().Split('=', 2);
                    if (parts.Length == 2)
                    {
                        cookieDict[parts[0]] = parts[1];
                    }
                }

                request.Headers.Add("Cookie", savedCookies);
            }

            var response = await base.SendAsync(request, cancellationToken);

            // 2. Merge any new Set-Cookie headers returned by the API
            if (response.Headers.TryGetValues("Set-Cookie", out var setCookies) && context != null)
            {
                foreach (var header in setCookies)
                {
                    var pair = header.Split(';')[0].Trim();
                    var parts = pair.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        cookieDict[parts[0]] = parts[1];
                    }
                }

                var merged = string.Join("; ", cookieDict.Select(kvp => $"{kvp.Key}={kvp.Value}"));
                context.Session.SetString("ApiCookies", merged);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && context != null)
            {
                context.Session.Remove("ApiCookies");
            }

            return response;
        }
    }
}
