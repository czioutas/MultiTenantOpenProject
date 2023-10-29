namespace MultiTenantOpenProject.API.Exceptions;

///<Summary>
/// Exception Type for handled cases.
///</Summary>
public class ResourceNotModifiedException : Exception
{
    ///<Summary>
    /// Parameter-less constructor
    ///</Summary>
    public ResourceNotModifiedException(string hash) : base($"Resource not modified for hash [{hash}].") { }
}