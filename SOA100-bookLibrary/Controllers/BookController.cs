using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOA100_bookLibrary.Data;
using SOA100_bookLibrary.DTOs;
using SOA100_bookLibrary.DTOs.Books;

namespace SOA100_bookLibrary.Controllers
{
    [ApiController] 
    [Route("[controller]")]
    
    public class BookController : ControllerBase
    {
        private readonly BookLibraryDbContext _dbContext;
        
        public BookController(BookLibraryDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        //Hämta ut böcker med detaljer (id, titel, år, författarnamn och kategorinamn
        //Endpoint blir /book/with-details (READ)
        [HttpGet("with-details")]
        public async Task<ActionResult<List<BookListDTO>>> GetBooksWithDetails()
        {
            var books = await _dbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Select(b => new BookListDTO(
                    b.Id,
                    b.Title,
                    b.Year,
                    b.Author.Name,
                    b.Category.Name
                    ))
                .ToListAsync();
            
            return Ok(books); //Ok ger ett 200 response.
                
        }

        //Hämta ut bok efter ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookDetailsDTO>> GetBookById(int id)
        {
            var book = await _dbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Where(b => b.Id == id)
                .Select(b => new BookDetailsDTO(
                    b.Id,
                    b.Title,
                    b.Year,
                    b.Author.Id,
                    b.Author.Name,
                    b.Category.Id,
                    b.Category.Name
                    ))
                .FirstOrDefaultAsync();
            
            if (book == null)
                return NotFound();
            
            return Ok(book);
        }
        

        //Skapa en bok, kontrollera Author och Category
        [HttpPost]
        public async Task<ActionResult<BookCreateDTO>> CreateBook([FromBody] BookCreateDTO dto)
        {
            //kontrollera Foreign Key relationerna
            var authorExists = await _dbContext.Authors.AnyAsync(a => a.Id == dto.AuthorId);
            if (!authorExists) return BadRequest($"AuthorId {dto.AuthorId} Finns inte!");
            
            var categoryExists = await _dbContext.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists) return BadRequest($"CategoryId {dto.CategoryId} Finns inte!");
            
            //skapar boken
            var book = new Book
            {
                Title = dto.Title,
                Year = dto.Year,
                AuthorId = dto.AuthorId,
                CategoryId = dto.CategoryId
            };
            
            _dbContext.Books.Add(book); //Lägger till boken
            await _dbContext.SaveChangesAsync(); //Sparar boken
            
            //Returnerar en ny DTO med namn
            var created = await _dbContext.Books 
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Where(b => b.Id == book.Id)
                .Select(b => new BookDetailsDTO(
                    b.Id,
                    b.Title,
                    b.Year,
                    b.AuthorId,
                    b.Author.Name,
                    b.CategoryId,
                    b.Category.Name
                    ))
                .FirstOrDefaultAsync();
            
            return CreatedAtAction(nameof(GetBookById), new { id = created.Id }, created);
        }
        
        
        //UPDATE bok:
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] BookUpdateDTO dto)
        {
            var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null) return NotFound();
            
            //Kollar Foreign Key:
            var authorExists = await _dbContext.Authors.AnyAsync(a => a.Id == dto.AuthorId);
            if (!authorExists) return BadRequest($"AuthorId {dto.AuthorId} Finns inte!");
            
            var CategoryExists = await _dbContext.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!CategoryExists) return BadRequest($"CategoryId {dto.CategoryId} Finns inte!");
            
            //Uppdatera boken
            book.Title = dto.Title;
            book.Year = dto.Year;
            book.AuthorId = dto.AuthorId;
            book.CategoryId = dto.CategoryId;
            
            await _dbContext.SaveChangesAsync();
            
            return NoContent(); //det gick bra att uppdatera boken
        }
        
        //DELETE bok:
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null) return NotFound();
            
            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync();
            
            return NoContent(); //Det gick bra att radera boken
            
        }
        
    }
}
