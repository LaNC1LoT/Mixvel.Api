using Mixvel.Api.Core.Models;
using Route = Mixvel.Api.Core.Models.Route;

namespace Mixvel.Api.Core.Interfaces;

public interface IProviderAdapter
{
    Task<Route[]> SearchAsync(SearchRequest query, CancellationToken cancellationToken);
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken);
}