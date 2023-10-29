using System.Collections.Generic;
using System.Linq;
using MultiTenantOpenProject.API.Models;

namespace MultiTenantOpenProject.API.Exceptions;

/// <summary>
/// 
/// </summary>
public class ServiceException : System.Exception
{
    private readonly List<ServiceError> _serviceErrors;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceError"></param>
    /// <returns></returns>
    public ServiceException(ServiceError serviceError) : base("Service Exception")
    {
        _serviceErrors = new List<ServiceError>();
        _serviceErrors.Add(serviceError);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceErrors"></param>
    /// <returns></returns>
    public ServiceException(IEnumerable<ServiceError> serviceErrors) : base("Service Exception")
    {
        _serviceErrors = serviceErrors.ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ServiceError> GetErrors()
    {
        return this._serviceErrors;
    }
}