namespace MultiTenantOpenProject.API.Exceptions;

///<Summary>
/// Exception Type for handled cases.
///</Summary>
public class ResourceNotFoundException : Exception
{
    ///<Summary>
    /// Parameter-less constructor
    ///</Summary>
    public ResourceNotFoundException() : base("Resource not found.") { }
}