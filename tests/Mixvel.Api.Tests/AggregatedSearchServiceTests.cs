using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mixvel.Api.Core.Interfaces;
using Mixvel.Api.Core.Models;
using Mixvel.Api.Infrastructure.Aggregators;
using Mixvel.Api.Options;
using NSubstitute;
using System.Text.Json;

namespace Mixvel.Api.Tests;

public class AggregatedSearchServiceTests
{
    private readonly IFixture _fixture;
    private readonly ICacheService _cacheService;
    private readonly IOptions<CacheOptions> _options;
    private readonly ILogger<AggregatedSearchService> _logger;
    private readonly IList<IProviderAdapter> _providers;
    private readonly AggregatedSearchService _aggregatedSearchService;

    public AggregatedSearchServiceTests()
    {
        _fixture = new Fixture();
        _cacheService = Substitute.For<ICacheService>();
        _options = Substitute.For<IOptions<CacheOptions>>();
        _options.Value.Returns(new CacheOptions
        {
            CacheRouteLifeTimeInMls = 10000
        });
        _logger = Substitute.For<ILogger<AggregatedSearchService>>();
        _providers = [Substitute.For<IProviderAdapter>(), Substitute.For<IProviderAdapter>()];

        _aggregatedSearchService = new AggregatedSearchService(
            _providers,
            _cacheService,
            _options,
            _logger
        );
    }

    [Fact]
    public async Task SearchAsync_ShouldUseCache_WhenOnlyCachedFilterIsSet()
    {
        // Arrange
        var request = _fixture.Create<SearchRequest>();
        var routes = _fixture.Create<Route[]>();
        request.Filters = new SearchFilters
        {
            OnlyCached = true
        };

        // Act
        var result = await _aggregatedSearchService.SearchAsync(request, CancellationToken.None);
        _providers[0].SearchAsync(request, Arg.Any<CancellationToken>()).Returns(routes);

        // Assert
        _cacheService.Received(1).TryGetValue(Arg.Any<string>(), out Arg.Any<Route[]>()!);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task SearchAsync_ShouldCacheResults_WhenNotCached()
    {
        // Arrange
        var request = _fixture.Create<SearchRequest>();
        request.Filters = new SearchFilters
        {
            OnlyCached = false
        };

        var routes = new[]
        {
            new Route
            {
                DestinationDateTime = DateTime.Now.AddDays(1),
                Price = 50.0m,
                TimeLimit = DateTime.Now.AddHours(1)
            },
            new Route
            {
                DestinationDateTime = DateTime.Now.AddDays(2),
                Price = 150.0m,
                TimeLimit = DateTime.Now.AddHours(2)
            }
        };

        _providers[0].SearchAsync(request, Arg.Any<CancellationToken>()).Returns(routes);

        // Act
        var result = await _aggregatedSearchService.SearchAsync(request, CancellationToken.None);

        // Assert
        // Проверяем, что результат кэшируется
        var cacheKey = Arg.Any<string>(); // Пример, как будет генерироваться ключ
        _cacheService.Received(1).Set(cacheKey, Arg.Any<IEnumerable<Route>>(), Arg.Any<TimeSpan>());
        // Проверяем, что результат не пустой
        result.Should().NotBeNull();
        result.Routes.Should().HaveCount(2);  // Проверяем, что возвращены маршруты
    }

    [Theory]
    [ClassData(typeof(FilterTestCases))]
    public async Task SearchAsync_ShouldApplyFilters_WhenFiltersAreSet(
        string serializedFilters,
        string serializedRoutes,
        string serializedExpectedRoutes)
    {
        // Arrange
        var filters = JsonSerializer.Deserialize<SearchFilters>(serializedFilters);
        var routes = JsonSerializer.Deserialize<Route[]>(serializedRoutes)!;
        var expectedRoutes = JsonSerializer.Deserialize<Route[]>(serializedExpectedRoutes);

        var request = _fixture.Create<SearchRequest>();
        request.Filters = filters;
        _providers[0].SearchAsync(request, Arg.Any<CancellationToken>()).Returns(routes);

        // Act
        var result = await _aggregatedSearchService.SearchAsync(request, CancellationToken.None);

        // Assert
        result.Routes.Should().BeEquivalentTo(expectedRoutes, options => options.WithStrictOrdering());
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public async Task IsAvailableAsync_ShouldReturnTrue_WhenAtLeastOneProviderIsAvailable(bool one, bool two)
    {
        // Arrange
        _providers[0].IsAvailableAsync(Arg.Any<CancellationToken>()).Returns(one);
        _providers[1].IsAvailableAsync(Arg.Any<CancellationToken>()).Returns(two);

        // Act
        var result = await _aggregatedSearchService.IsAvailableAsync(CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsAvailableAsync_ShouldReturnFalse_WhenNoProvidersAreAvailable()
    {
        // Arrange
        _providers[0].IsAvailableAsync(Arg.Any<CancellationToken>()).Returns(false);
        _providers[1].IsAvailableAsync(Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _aggregatedSearchService.IsAvailableAsync(CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }
}
