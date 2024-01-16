using System.Net;
using AutoFixture;
using MongoDB.Driver;
using SuitStore.Alterations.Core.Models;
using SuitStore.Alterations.Core.Saga;

namespace SuitStore.Alterations.Test.Endpoints.Alterations;

[Collection(ComponentTestsCollection.Name)]
public class GetByTailorIdTests : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly IMongoCollection<AlterationSaga> _alterationsCollection;
    private readonly Fixture _fixture = new();

    public GetByTailorIdTests(WebAppFactory webAppFactory)
    {
        _httpClient = webAppFactory.CreateClient();
        _alterationsCollection = webAppFactory.AlterationsCollection;
    }

    [Fact(DisplayName = "Execute returns only the alterations that the provided tailor is working on")]
    public async Task Execute_ReturnsFilteredAlterations_WhenTailorIdProvided()
    {
        var tailorId = _fixture.Create<long>();
        var alterationWithTailorId = _fixture.Create<Guid>();

        var alterationsToBeInserted = new List<AlterationSaga>()
        {
            _fixture.Build<AlterationSaga>()
                .With(a => a.TailorId, tailorId)
                .With(a => a.AlterationId, alterationWithTailorId).Create(),
            _fixture.Create<AlterationSaga>()
        };

        await _alterationsCollection.InsertManyAsync(alterationsToBeInserted);

        using var response = await _httpClient.GetAsync($"v1/tailors/{tailorId}/alterations");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var alterations = await response.Content.ReadAsAsync<IEnumerable<Alteration>>();
        
        Assert.Single(alterations);
        Assert.Equal(alterationWithTailorId, alterations.Single().AlterationId);
        Assert.Equal(tailorId, alterations.Single().TailorId);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}