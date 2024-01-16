using System.Net;
using AutoFixture;
using MongoDB.Driver;
using SuitStore.Alterations.Api.Requests;
using SuitStore.Alterations.Core.Models;
using SuitStore.Alterations.Core.Saga;

namespace SuitStore.Alterations.Test.Endpoints.Alterations;

[Collection(ComponentTestsCollection.Name)]
public class CreateTests : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly IMongoCollection<AlterationSaga> _alterationsCollection;
    private readonly Fixture _fixture = new();

    public CreateTests(WebAppFactory webAppFactory)
    {
        _httpClient = webAppFactory.CreateClient();
        _alterationsCollection = webAppFactory.AlterationsCollection;
    }

    [Fact(DisplayName = "Execute creates a new saga when submit is successful")]
    public async Task Execute_CreatesNewSaga_WhenSubmitIsSuccessful()
    {
        var alterationInstructionList = new List<AlterationInstruction>()
        {
            new(AlterationType.LeftSleeve, 5)
        };

        var rnd = new Random();
        var clientId = rnd.NextInt64(long.MaxValue);
        var productId = _fixture.Create<long>();
        var request = new CreateAlterationRequest(alterationInstructionList, productId);
        
        using var response = await _httpClient.PostAsJsonAsync($"v1/clients/{clientId}/alterations", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        // TODO: Use observers instead
        await Task.Delay(TimeSpan.FromSeconds(1));

        var alteration = await _alterationsCollection.Find(a => a.ClientId == clientId).SingleAsync();
        
        Assert.Equal(productId, alteration.ProductId);
        Assert.Equal(clientId, alteration.ClientId);
        Assert.Single(alteration.AlterationInstructions);
        Assert.Equal(alterationInstructionList.Single().Type, alteration.AlterationInstructions.Single().Type);
        Assert.Equal(alterationInstructionList.Single().ChangeInCm, alteration.AlterationInstructions.Single().ChangeInCm);
        Assert.Equal(nameof(AlterationStateMachine.AwaitingPayment), alteration.CurrentState);
    }
    
    [Theory(DisplayName = "Execute returns BadRequest when change is larger or smaller than 5cm")]
    [InlineData(-6)]
    [InlineData(6)]
    public async Task Execute_ReturnsBadRequest_WhenAlterationIsTooLarge(int changeInCm)
    {
        var alterationInstructionList = new List<AlterationInstruction>()
        {
            new(AlterationType.LeftSleeve, changeInCm)
        };

        var rnd = new Random();
        var clientId = rnd.NextInt64(long.MaxValue);
        var productId = _fixture.Create<long>();
        var request = new CreateAlterationRequest(alterationInstructionList, productId);
        
        using var response = await _httpClient.PostAsJsonAsync($"v1/clients/{clientId}/alterations", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact(DisplayName = "Execute returns BadRequest when the same alteration type is posted twice")]
    public async Task Execute_ReturnsBadRequest_WhenAlterationHasInvalidSetOfChanges()
    {
        var alterationInstructionList = new List<AlterationInstruction>()
        {
            new(AlterationType.LeftSleeve, 5),
            new(AlterationType.LeftSleeve, 2)
        };

        var rnd = new Random();
        var clientId = rnd.NextInt64(long.MaxValue);
        var productId = _fixture.Create<long>();
        var request = new CreateAlterationRequest(alterationInstructionList, productId);
        
        using var response = await _httpClient.PostAsJsonAsync($"v1/clients/{clientId}/alterations", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}