namespace MultiTenantOpenProject.API.Exceptions;

/// <summary>
/// 
/// </summary>
public class InvalidContractException : Exception
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public InvalidContractException() : base()
    {
    }

    ///<Summary>
    /// Message only constructor
    ///</Summary>
    public InvalidContractException(string message) : base(message) { }
}
