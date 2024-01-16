using System.Net;
using AutoFixture;
using MongoDB.Driver;
using SuitStore.Alterations.Core.Models;
using SuitStore.Alterations.Core.Saga;

namespace SuitStore.Alterations.Test.Endpoints.Alterations;

[Collection(ComponentTestsCollection.Name)]
public class GetTests : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly IMongoCollection<AlterationSaga> _alterationsCollection;
    private readonly Fixture _fixture = new();

    public GetTests(WebAppFactory webAppFactory)
    {
        _httpClient = webAppFactory.CreateClient();
        _alterationsCollection = webAppFactory.AlterationsCollection;
    }

    [Fact(DisplayName = "Execute returns only the alterations that the provided tailor is working on")]
    public async Task Execute_ReturnsFilteredAlterations_WhenTailorIdProvided()
    {
        var rnd = new Random();
        var tailorId = rnd.NextInt64(long.MaxValue);
        var alterationWithTailorId = _fixture.Create<Guid>();

        var alterationsToBeInserted = new List<AlterationSaga>()
        {
            _fixture.Build<AlterationSaga>()
                .With(a => a.TailorId, tailorId)
                .With(a => a.AlterationId, alterationWithTailorId).Create(),
            _fixture.Create<AlterationSaga>()
        };

        await _alterationsCollection.InsertManyAsync(alterationsToBeInserted);

        using var response = await _httpClient.GetAsync($"v1/alterations?tailorId={tailorId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var alterations = await response.Content.ReadAsAsync<IEnumerable<Alteration>>();
        
        Assert.Single(alterations);
        Assert.Equal(alterationWithTailorId, alterations.Single().AlterationId);
        Assert.Equal(tailorId, alterations.Single().TailorId);
    }
    
    [Fact(DisplayName = "Execute returns only the alterations that are in a specified state")]
    public async Task Execute_ReturnsFilteredAlterations_WhenStateIsProvided()
    {
        var alterationId = _fixture.Create<Guid>();
        var state = nameof(AlterationStateMachine.AwaitingPayment);
            
        var alterationsToBeInserted = new List<AlterationSaga>()
        {
            _fixture.Build<AlterationSaga>()
                .With(a => a.CurrentState, state)
                .With(a => a.AlterationId, alterationId).Create(),
            _fixture.Create<AlterationSaga>()
        };

        await _alterationsCollection.InsertManyAsync(alterationsToBeInserted);

        using var response = await _httpClient.GetAsync($"v1/alterations?state={state}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var alterations = await response.Content.ReadAsAsync<IEnumerable<Alteration>>();
        
        Assert.All(alterations, a => Assert.Equal(state, a.CurrentState));
        Assert.Contains(alterations, a => a.AlterationId == alterationId);
    }
    
    [Fact(DisplayName = "Execute returns only the alterations that are in a specified state and belong to specific tailor")]
    public async Task Execute_ReturnsFilteredAlterations_WhenStateAndTailorIdAreProvided()
    {
        var alterationId = _fixture.Create<Guid>();
        var state = nameof(AlterationStateMachine.AwaitingPayment);
        var rnd = new Random();
        var tailorId = rnd.NextInt64(long.MaxValue);

        var alterationsToBeInserted = new List<AlterationSaga>()
        {
            _fixture.Build<AlterationSaga>()
                .With(a => a.CurrentState, state)
                .With(a => a.TailorId, tailorId)
                .With(a => a.AlterationId, alterationId).Create(),
            _fixture.Create<AlterationSaga>()
        };

        await _alterationsCollection.InsertManyAsync(alterationsToBeInserted);

        using var response = await _httpClient.GetAsync($"v1/alterations?state={state}&tailorId={tailorId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var alterations = await response.Content.ReadAsAsync<IEnumerable<Alteration>>();
        
        Assert.Single(alterations);
        Assert.Equal(alterationId, alterations.Single().AlterationId);
        Assert.Equal(tailorId, alterations.Single().TailorId);
        Assert.Equal(state, alterations.Single().CurrentState);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}