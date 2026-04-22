namespace Ecommerce.CatalogService.Domain.Exceptions
{
    public class EntityNotFoundException(string entityName, string entityId) 
        : DomainException($"Entity \"{entityName}\" ({entityId}) was not found.")
    {
    }
}
