namespace MultiTenantOpenProject.API.Exceptions;

///<Summary>
/// Exception Type for handled cases.
///</Summary>
public class LogicException : Exception
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public LogicException(string message) : base(message)
    {
    }
}