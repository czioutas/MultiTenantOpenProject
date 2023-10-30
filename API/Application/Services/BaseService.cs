namespace MultiTenantOpenProject.API.Application.Services;

public class BaseService<SERVICE>
{
    private readonly ILogger<SERVICE> _logger;

    public BaseService(ILogger<SERVICE> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
