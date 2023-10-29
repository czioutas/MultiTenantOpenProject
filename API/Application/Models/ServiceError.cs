namespace MultiTenantOpenProject.API.Models;
/// <summary>
/// 
/// </summary>
public class ServiceError
{
    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public string ErrorMessage { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="errorMessage"></param>
    public ServiceError(string errorMessage)
    {
        this.ErrorMessage = errorMessage;
    }
}