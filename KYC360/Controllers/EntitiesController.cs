using KYC360.Models;
using KYC360.Repository;
using Microsoft.AspNetCore.Mvc;

namespace KYC360.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntitiesController : ControllerBase
    {
        private readonly IEntityRepository _repository;

        public EntitiesController(IEntityRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetAllEntities([FromQuery] string? search, [FromQuery] string? gender, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string[]? countries, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = "Id", [FromQuery] string sortOrder = "asc")
        {
            var entities = _repository.SearchEntities(search, gender, startDate, endDate, countries, pageNumber, pageSize, sortBy, sortOrder);
            if (entities.Count() == 0)
            {
                return NotFound();
            }
            
            return Ok(entities);
        }

        [HttpGet("{id}")]
        public IActionResult GetEntityById([FromRoute] string id)
        {
            var entity = _repository.GetEntityById(id);
            if (entity != null)
            {
                return Ok(entity);
            }
            
            return NotFound();
        }

        [HttpPost]
        public IActionResult CreateEntity([FromBody] Entity entity)
        {
            _repository.AddEntity(entity);
            return CreatedAtAction(nameof(GetEntityById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEntity([FromRoute] string id, [FromBody] Entity entity)
        {
            var existingEntity = _repository.GetEntityById(id);
            if (existingEntity == null)
            {
                return NotFound();
            }

            _repository.UpdateEntity(entity);
            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEntity([FromRoute] string id)
        {
            var existingEntity = _repository.GetEntityById(id);
            if (existingEntity == null)
            {
                return NotFound();
            }

            _repository.DeleteEntity(id);
            return Ok(id);
        }
    }
}