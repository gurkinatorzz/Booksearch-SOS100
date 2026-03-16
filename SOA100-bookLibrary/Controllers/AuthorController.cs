using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOA100_bookLibrary.Data;
using SOA100_bookLibrary.DTOs.Authors;

namespace SOA100_bookLibrary.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly BookLibraryDbContext _dbContext;
        
        public AuthorController(BookLibraryDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        //READ authors
        [HttpGet]
        public async Task<ActionResult<List<AuthorListDTO>>> GetAuthors()
        {
            var authors = await _dbContext.Authors
                .Select(a => new AuthorListDTO(a.Id, a.Name))
                .ToListAsync();
            
            return Ok(authors); //returnerar OK
        }

        //READ en specifik author (ID)
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AuthorListDTO>> GetAuthorById(int id)
        {
            var author = await _dbContext.Authors
                .Where(a => a.Id == id)
                .Select(a => new AuthorListDTO(a.Id, a.Name))
                .FirstAsync();
            
            if (author == null) return NotFound();
            return Ok(author);
        }
        
        //CREATE author
        [HttpPost]
        public async Task<ActionResult<AuthorListDTO>> CreateAuthor([FromBody] AuthorCreateDTO dto)
        {
            var name = dto.Name.Trim();

            var exists = await _dbContext.Authors.AnyAsync(a => a.Name.ToLower() == name.ToLower());

            if (exists)
                return Conflict("En författare med samma namn finns redan"); //undvik dublett
            
            var author = new Author { Name = name };
            
            _dbContext.Authors.Add(author);
            await _dbContext.SaveChangesAsync();
            
            var created = new AuthorListDTO(author.Id, author.Name);
            return CreatedAtAction(nameof(GetAuthorById), new { id = created.Id }, created);
        }
        
        //UPDATE author
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] AuthorCreateDTO dto)
        {
            var author = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);
            if (author == null)  return NotFound();
            
            var name = dto.Name.Trim();

            var exists = await _dbContext.Authors
                .AnyAsync(a => a.Id != id && a.Name.ToLower() == name.ToLower());

            if (exists)
                return Conflict("En författare med samma namn finns redan.");
            
            author.Name = dto.Name;
            await _dbContext.SaveChangesAsync();
            
            return NoContent();
        }
        
        //DELETE author
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);
            if (author == null) return NotFound();
            
            //Skydda foreign key relationer
            var isUsed = await _dbContext.Books.AnyAsync(b => b.CategoryId == id);
            if (isUsed)
                return Conflict("Kan inte ta bort författaren - den används av en eller flera böcker");
            
            _dbContext.Authors.Remove(author);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        
    }
}
