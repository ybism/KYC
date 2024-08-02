using KYC360.Helpers;
using KYC360.Models;

namespace KYC360.Repository;

public class EntityRepository : IEntityRepository
{
    private List<Entity> entities = new List<Entity>
    {
        new Entity
        {
            Id = "1",
            Names = new List<Name> { new Name { FirstName = "Yusuf", MiddleName = "Bismillah" } },
            Addresses = new List<Address> { new Address { City = "London", Country = "UK" } },
            Dates = new List<Dates> { new Dates { Date = new DateTime(2024, 1, 1), DateType = "Birthday" } },
            Deceased = false,
            Gender = "Male"
        },
        new Entity
        {
            Id = "2",
            Names = new List<Name> { new Name { FirstName = "Yusuf", MiddleName = "Bismillah" } },
            Addresses = new List<Address> { new Address { City = "Southampton", Country = "UK" } },
            Dates = new List<Dates> { new Dates { Date = new DateTime(2024, 2, 1), DateType = "Birthday" } },
            Deceased = false,
            Gender = "Female"
        }
    };


    public IEnumerable<Entity> GetAllEntities() => entities;

    public Entity GetEntityById(string id) => entities.FirstOrDefault(x => x.Id == id);

    public void AddEntity(Entity entity)
    {
        RetryHelper.ExecuteWithRetry(() => entities.Add(entity), 3, 1000);
    }

    public IEnumerable<Entity> SearchEntities(string? searchTerm, string? gender, DateTime? startDate,
        DateTime? endDate, string[]? countries, int pageNumber, int pageSize, string? sortBy, string sortOrder)
    {
        var searchTerms = string.IsNullOrEmpty(searchTerm)
            ? new string[0]
            : searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var filteredEntities = entities.Where(e =>
            (string.IsNullOrEmpty(gender) || e.Gender == gender) &&
            (!startDate.HasValue || e.Dates.Any(d => d.Date >= startDate)) &&
            (!endDate.HasValue || e.Dates.Any(d => d.Date <= endDate)) &&
            (countries == null || countries.Length == 0 || e.Addresses.Any(a => countries.Contains(a.Country))) &&
            (searchTerms.Length == 0 || searchTerms.Any(term =>
                e.Names.Any(n =>
                    (!string.IsNullOrEmpty(n.FirstName) &&
                     n.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(n.MiddleName) &&
                     n.MiddleName.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(n.Surname) && n.Surname.Contains(term, StringComparison.OrdinalIgnoreCase))
                ) ||
                e.Addresses.Any(a =>
                    (!string.IsNullOrEmpty(a.AddressLine) &&
                     a.AddressLine.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(a.Country) && a.Country.Contains(term, StringComparison.OrdinalIgnoreCase))
                )
            ))
        );

        // Sorting
        filteredEntities = sortOrder.ToLower() == "desc"
            ? filteredEntities.OrderByDescending(e => GetPropertyValue(e, sortBy))
            : filteredEntities.OrderBy(e => GetPropertyValue(e, sortBy));

        // Pagination
        return filteredEntities
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public Entity UpdateEntity(Entity entity)
    {
        Entity entityToUpdate = null;
        RetryHelper.ExecuteWithRetry(() =>
        {
            entityToUpdate = entities.FirstOrDefault(x => x.Id == entity.Id);
            if (entityToUpdate != null)
            {
                entities.Remove(entityToUpdate);
                entities.Add(entity);
            }
        }, 3, 1000);

        return entityToUpdate != null ? entity : null;
    }

    public string DeleteEntity(string id)
    {
        Entity entityToDelete = null;
        RetryHelper.ExecuteWithRetry(() =>
        {
            entityToDelete = entities.FirstOrDefault(x => x.Id == id);
            if (entityToDelete != null)
            {
                entities.Remove(entityToDelete);
            }
        }, 3, 1000);

        return entityToDelete != null ? id : null;
    }

    private object? GetPropertyValue(Entity entity, string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            return entity.Id;

        var property = typeof(Entity).GetProperty(propertyName);
        return property != null ? property.GetValue(entity) : entity.Id;
    }
}