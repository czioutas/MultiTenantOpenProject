namespace MultiTenantOpenProject.API.Exceptions;

///<Summary>
/// Exception Type for external services.
///</Summary>
public class ExternalServiceException : Exception
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public ExternalServiceException(string message) : base(message)
    {
    }
}