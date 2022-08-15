namespace WebApplication1.Services;

public class HarborService
{
    private readonly IServiceCollection _services;

    HarborService(IServiceCollection services)
    {
        _services = services;
    }

    public void GetProductInfo()
    {
        _services.AddHttpClient();
    }
}
