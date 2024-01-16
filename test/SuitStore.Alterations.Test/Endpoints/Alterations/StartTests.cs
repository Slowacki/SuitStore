using System.Net;
using AutoFixture;
using MongoDB.Driver;
using SuitStore.Alterations.Api.Requests;
using SuitStore.Alterations.Core.Saga;

namespace SuitStore.Alterations.Test.Endpoints.Alterations;

[Collection(ComponentTestsCollection.Name)]
public class StartTests : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly IMongoCollection<AlterationSaga> _alterationsCollection;
    private readonly Fixture _fixture = new();

    public StartTests(WebAppFactory webAppFactory)
    {
        _httpClient = webAppFactory.CreateClient();
        _alterationsCollection = webAppFactory.AlterationsCollection;
    }

    [Fact(DisplayName = "Execute transitions an alteration from AwaitingForPayment state to InProgress state")]
    public async Task Execute_TransitionsAlteration_WhenSubmitIsSuccessful()
    {
        var alterationId = _fixture.Create<Guid>();
        var tailorId = _fixture.Create<long>();
        var alterationToBeInserted = _fixture.Build<AlterationSaga>()
            .With(a => a.CurrentState, nameof(AlterationStateMachine.ReadyToStart))
            .With(a => a.TailorId, tailorId)
            .With(a => a.AlterationId, alterationId).Create();

        await _alterationsCollection.InsertOneAsync(alterationToBeInserted);

        var request = new StartAlterationRequest(tailorId);
        
        using var response = await _httpClient.PostAsJsonAsync($"v1/alterations/{alterationId}/start", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        // TODO: Use observers instead
        await Task.Delay(TimeSpan.FromSeconds(1));

        var alteration = await _alterationsCollection.Find(a => a.AlterationId == alterationId).SingleAsync();
        
        Assert.Equal(alterationId, alteration.AlterationId);
        Assert.Equal(tailorId, alteration.TailorId);
        
        Assert.Equal(nameof(AlterationStateMachine.InProgress), alteration.CurrentState);
    }
    
    [Theory(DisplayName = "Execute returns BadRequest when in state that is not ReadyToStart")]
    [InlineData(nameof(AlterationStateMachine.InProgress))]
    [InlineData(nameof(AlterationStateMachine.Completed))]
    [InlineData(nameof(AlterationStateMachine.AwaitingPayment))]
    public async Task Execute_ReturnsBadRequest_WhenStateIsNotReadyToStart(string currentState)
    {
        var alterationId = _fixture.Create<Guid>();
        var tailorId = _fixture.Create<long>();
        var alterationToBeInserted = _fixture.Build<AlterationSaga>()
            .With(a => a.CurrentState, currentState)
            .With(a => a.TailorId, tailorId)
            .With(a => a.AlterationId, alterationId).Create();

        await _alterationsCollection.InsertOneAsync(alterationToBeInserted);

        var request = new StartAlterationRequest(tailorId);
        
        using var response = await _httpClient.PostAsJsonAsync($"v1/alterations/{alterationId}/start", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact(DisplayName = "Returns NotFound when alteration with given id cannot be found")]
    public async Task Execute_ReturnsNotFound_WhenAlterationWithGivenIdDoesNotExist()
    {
        var alterationId = _fixture.Create<Guid>();
        var tailorId = _fixture.Create<long>();

        var request = new StartAlterationRequest(tailorId);
        
        using var response = await _httpClient.PostAsJsonAsync($"v1/alterations/{alterationId}/start", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}