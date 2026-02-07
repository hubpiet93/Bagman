using ErrorOr;

namespace Bagman.Application.Common;

public interface IFeatureHandler<in TRequest, TResponse>
{
    Task<ErrorOr<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}
