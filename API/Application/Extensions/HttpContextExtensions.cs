using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MultiTenantOpenProject.API.Application;
using Newtonsoft.Json.Linq;

namespace MultiTenantOpenProject.API.Extensions;

public static class HttpContextExtensions
{
    public static Task WriteHealthReportResponse(HttpContext httpContext, HealthReport result)
    {
        httpContext.Response.ContentType = MediaTypeNames.Application.Json;

        var json = new JObject(
            new JProperty("status", result.Status.ToString()),
            new JProperty("results", new JObject(result.Entries.Select(pair =>
                new JProperty(pair.Key, new JObject(
                    new JProperty("status", pair.Value.Status.ToString()),
                    new JProperty("description", pair.Value.Description),
                    new JProperty("data", new JObject(pair.Value.Data.Select(
                        p => new JProperty(p.Key, p.Value))))))))));
        return httpContext.Response.WriteAsync(
            json.ToString((Newtonsoft.Json.Formatting)Formatting.Indented));
    }

    public static Task WritePrometheusHealthReport(HttpContext httpContext, HealthReport result)
    {
        httpContext.Response.ContentType = MediaTypeNames.Text.Plain;
        var stringBuilder = new StringBuilder();
        foreach (var entry in result.Entries)
        {
            stringBuilder.Append(entry.Key.ToSnakeCase() + " " + (int)entry.Value.Status + "\n");
        }

        return httpContext.Response.WriteAsync(stringBuilder.ToString());
    }

    public static Guid GetUserId(this HttpContext httpContext)
    {
        string _key = ClaimTypeConstants.UserId;

        if (!Guid.TryParse(GetClaimValue(httpContext, _key), out Guid x))
        {
            return default;
        }

        return x;
    }

    private static string? GetClaimValue(HttpContext context, string ClaimType)
    {

        context.Request.Headers.TryGetValue("Authorization", out var accessToken);

        if (string.IsNullOrEmpty(accessToken))
        {
            return null;
        }

        accessToken = accessToken.ToString().Replace("Bearer", "").Trim();

        if (string.IsNullOrEmpty(accessToken))
        {
            return null;
        }

        JwtSecurityToken jwt = new JwtSecurityToken(accessToken);

        return jwt.Claims.First(c => c.Type == ClaimType).Value;
    }
}
