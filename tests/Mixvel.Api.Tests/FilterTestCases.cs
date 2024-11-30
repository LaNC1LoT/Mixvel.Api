using Mixvel.Api.Core.Models;
using System.Text.Json;

namespace Mixvel.Api.Tests;

public class FilterTestCases : TheoryData<string, string, string>
{
    private static readonly DateTime StaticDateTime = new(2024, 12, 1, 12, 0, 0); // Статическое время

    public FilterTestCases()
    {
        // Фильтры заданы, маршрут проходит по всем фильтрам
        Add(
            JsonSerializer.Serialize(new SearchFilters
            {
                DestinationDateTime = StaticDateTime.AddDays(3),
                MaxPrice = 100.0m,
                MinTimeLimit = StaticDateTime.AddHours(1)
            }),
            JsonSerializer.Serialize(new[]
            {
                new Route
                {
                    DestinationDateTime = StaticDateTime.AddDays(2),
                    Price = 50.0m,
                    TimeLimit = StaticDateTime.AddHours(2)
                },
                new Route
                {
                    DestinationDateTime = StaticDateTime.AddDays(3),
                    Price = 150.0m,
                    TimeLimit = StaticDateTime.AddHours(3)
                }
            }),
            JsonSerializer.Serialize(new[]
            {
                new Route
                {
                    DestinationDateTime = StaticDateTime.AddDays(2),
                    Price = 50.0m,
                    TimeLimit = StaticDateTime.AddHours(2)
                }
            })
        );

        // Фильтры не заданы, все маршруты должны остаться
        Add(
            JsonSerializer.Serialize(new SearchFilters
            {
                DestinationDateTime = null,
                MaxPrice = null,
                MinTimeLimit = null
            }),
            JsonSerializer.Serialize(new[]
            {
                new Route
                {
                    DestinationDateTime = StaticDateTime.AddDays(1),
                    Price = 50.0m,
                    TimeLimit = StaticDateTime.AddHours(1)
                },
                new Route
                {
                    DestinationDateTime = StaticDateTime.AddDays(2),
                    Price = 150.0m,
                    TimeLimit = StaticDateTime.AddHours(2)
                }
            }),
            JsonSerializer.Serialize(new[]
            {
                new Route
                {
                    DestinationDateTime = StaticDateTime.AddDays(1),
                    Price = 50.0m,
                    TimeLimit = StaticDateTime.AddHours(1)
                },
                new Route
                {
                    DestinationDateTime = StaticDateTime.AddDays(2),
                    Price = 150.0m,
                    TimeLimit = StaticDateTime.AddHours(2)
                }
            })
        );

        // Маршрут отфильтровывается по MaxPrice
        Add(
            JsonSerializer.Serialize(new SearchFilters
            {
                DestinationDateTime = null,
                MaxPrice = 100.0m,
                MinTimeLimit = null
            }),
            JsonSerializer.Serialize(new[]
            {
                new Route
                {
                    DestinationDateTime = StaticDateTime.AddDays(1),
                    Price = 50.0m,
                    TimeLimit = StaticDateTime.AddHours(1)
                },
                new Route
                {
                    DestinationDateTime = StaticDateTime.AddDays(2),
                    Price = 150.0m,
                    TimeLimit = StaticDateTime.AddHours(2)
                }
            }),
            JsonSerializer.Serialize(new[]
            {
                new Route
                {
                    DestinationDateTime = StaticDateTime.AddDays(1),
                    Price = 50.0m,
                    TimeLimit = StaticDateTime.AddHours(1)
                }
            })
        );

        // Маршрут отфильтровывается по DestinationDateTime
        Add(
            JsonSerializer.Serialize(new SearchFilters
            {
                DestinationDateTime = StaticDateTime.AddDays(1),
                MaxPrice = null,
                MinTimeLimit = null
            }),
            JsonSerializer.Serialize(new[]
            {
                new Route
                {
                    DestinationDateTime = StaticDateTime.AddDays(2),
                    Price = 50.0m,
                    TimeLimit = StaticDateTime.AddHours(1)
                },
                new Route
                {
                    DestinationDateTime = StaticDateTime.AddDays(1),
                    Price = 150.0m,
                    TimeLimit = StaticDateTime.AddHours(2)
                }
            }),
            JsonSerializer.Serialize(new[]
            {
                new Route
                {
                    DestinationDateTime = StaticDateTime.AddDays(1),
                    Price = 150.0m,
                    TimeLimit = StaticDateTime.AddHours(2)
                }
            })
        );

        // Маршрут отфильтровывается по MinTimeLimit
        Add(
            JsonSerializer.Serialize(new SearchFilters
            {
                DestinationDateTime = null,
                MaxPrice = null,
                MinTimeLimit = StaticDateTime.AddHours(2)
            }),
            JsonSerializer.Serialize(new[]
            {
                new Route
                {
                    DestinationDateTime = StaticDateTime.AddDays(1),
                    Price = 50.0m,
                    TimeLimit = StaticDateTime.AddHours(1)
                },
                new Route
                {
                    DestinationDateTime = StaticDateTime.AddDays(2),
                    Price = 150.0m,
                    TimeLimit = StaticDateTime.AddHours(2)
                }
            }),
            JsonSerializer.Serialize(new[]
            {
                new Route
                {
                    DestinationDateTime = StaticDateTime.AddDays(2),
                    Price = 150.0m,
                    TimeLimit = StaticDateTime.AddHours(2)
                }
            })
        );

        // Пустой список маршрутов
        Add(
            JsonSerializer.Serialize(new SearchFilters
            {
                DestinationDateTime = StaticDateTime.AddDays(3),
                MaxPrice = 100.0m,
                MinTimeLimit = StaticDateTime.AddHours(1)
            }),
            JsonSerializer.Serialize(Array.Empty<Route>()), // Пустой входной список
            JsonSerializer.Serialize(Array.Empty<Route>())  // Результат тоже пустой
        );
    }
}