using Microsoft.AspNetCore.Mvc;

namespace MultiTenantOpenProject.API.Models;
/// <summary>
/// A wrapper class for an Application Level exception, which includes helpful details.
/// </summary>
public class ApplicationProblemDetailsModel : ProblemDetails
{
    /// <summary>
    /// The request TraceId
    /// </summary>
    public string TraceId { get; set; }

    /// <summary>
    /// A Url pointing to the location of the logged error.
    /// </summary>
    public string LogUrl { get; set; }

    /// <summary>
    /// List of error messages.
    /// </summary>
    public List<string> Errors { get; set; } = new List<string>();

    /// <summary>
    /// Constructor for ApplicationProblemDetails
    /// </summary>
    /// <param name="statusCode">The HTTP Status Code</param>
    /// <param name="title">The exception error</param>
    /// <param name="exceptionType">The type of exception</param>
    /// <param name="path">The exception base path</param>
    /// <param name="traceId">The request TraceId</param>
    /// <param name="errors">List of added error messages</param>
    public ApplicationProblemDetailsModel(
        int statusCode,
        string title,
        string exceptionType,
        string path,
        string traceId,
        string[] errors)
    {
        Status = statusCode;
        Type = "https://httpstatuses.com/" + Status;
        Title = title;
        Detail = exceptionType;
        Instance = path;
        TraceId = traceId;
        LogUrl = "http://1.1.1.1/#/events?filter=RequestId" + System.Net.WebUtility.UrlEncode("=\"" + traceId + "\"");
        if (errors != null)
            Errors.AddRange(errors);
    }
}