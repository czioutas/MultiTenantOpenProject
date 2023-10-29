using Microsoft.AspNetCore.Identity;

namespace MultiTenantOpenProject.API.Exceptions;

///<Summary>
/// Exception Type for handled cases.
///</Summary>
public class IdentityException : Exception
{
    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<IdentityError> IdentityErrors;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="identityErrors"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public IdentityException(IEnumerable<IdentityError> identityErrors, string message = "Identity Error(s)") : base(message)
    {
        IdentityErrors = identityErrors;
    }
}