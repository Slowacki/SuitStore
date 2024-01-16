namespace SuitStore.Alterations.Test;

[CollectionDefinition(Name)]
public class ComponentTestsCollection : ICollectionFixture<WebAppFactory>
{
    public const string Name = "component tests";
}