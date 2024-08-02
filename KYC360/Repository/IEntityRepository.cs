using KYC360.Models;

namespace KYC360.Repository;

public interface IEntityRepository
{
    IEnumerable<Entity> GetAllEntities();
    Entity GetEntityById(string id);
    IEnumerable<Entity> SearchEntities(string? searchTerm, string? gender, DateTime? startDate, DateTime? endDate,
        string[]? countries, int pageNumber, int pageSize, string? sortBy, string sortOrder);
    void AddEntity(Entity entity);
    Entity UpdateEntity(Entity entity);
    string DeleteEntity(string id);
}