using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SOA100_bookLibrary.DTOs.Categories;
using SOA100_bookLibrary.Data;
using Microsoft.EntityFrameworkCore;

namespace SOA100_bookLibrary.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        private readonly BookLibraryDbContext _dbContext;

        public CategoryController(BookLibraryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //READ categories
        [HttpGet]
        public async Task<ActionResult<List<CategoryListDTO>>> GetCategories()
        {
            var categories = await _dbContext.Categories
                .Select(c => new CategoryListDTO(c.Id, c.Name))
                .ToListAsync();

            return Ok(categories); //returnerar OK
        }

        //READ en specifik category (ID)
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryListDTO>> GetCategoryById(int id)
        {
            var category = await _dbContext.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryListDTO(c.Id, c.Name))
                .FirstAsync();

            if (category == null) return NotFound();
            return Ok(category);
        }

        //CREATE category
        [HttpPost]
        public async Task<ActionResult<CategoryListDTO>> CreateCategory([FromBody] CategoryCreateDTO dto)
        {
            var name = dto.Name.Trim();
            
            var exists = await _dbContext.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower());

            if (exists)
                return Conflict("Kategorin finns redan");
            
            var category = new Category { Name = name };

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            var created = new CategoryListDTO(category.Id, category.Name);
            return CreatedAtAction(nameof(GetCategoryById), new { id = created.Id }, created);
        }

        //UPDATE category
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDTO dto)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            
            var name = dto.Name.Trim();

            var exists = await _dbContext.Categories
                .AnyAsync(c => c.Id != id && c.Name.ToLower() == name.ToLower());

            if (exists)
                return Conflict("En kategori med samma namn finns redan.");

            category.Name = dto.Name;
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        //DELETE category
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();

            //Skydda foreign key relationer
            var isUsed = await _dbContext.Books.AnyAsync(b => b.CategoryId == id);
            if (isUsed)
                return Conflict("Kan inte ta bort Kategorin - den används av en eller flera böcker");

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

    }

}