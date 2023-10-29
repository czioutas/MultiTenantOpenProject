using AutoMapper;

namespace MultiTenantOpenProject.API.Application.Services;

public class BaseService<SERVICE>
{
    public IMapper _mapper { get; set; }
    private readonly ILogger<SERVICE> _logger;

    public BaseService(ILogger<SERVICE> logger, IMapper mapper)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
}