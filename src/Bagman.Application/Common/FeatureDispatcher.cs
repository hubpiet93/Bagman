using ErrorOr;
using Microsoft.Extensions.DependencyInjection;

namespace Bagman.Application.Common;

public class FeatureDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public FeatureDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<ErrorOr<TResponse>> HandleAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        var handler = _serviceProvider.GetRequiredService<IFeatureHandler<TRequest, TResponse>>();
        return await handler.HandleAsync(request, cancellationToken);
    }
}
