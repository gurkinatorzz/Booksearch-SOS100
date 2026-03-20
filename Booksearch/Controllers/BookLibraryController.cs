using System.Runtime.Intrinsics.X86;
using Microsoft.AspNetCore.Mvc;
using Booksearch.Models.BookLibraryDtos;
using Booksearch.Services;
using Booksearch.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Booksearch.Controllers;

public class BookLibraryController : Controller
{
    
    private BookLibraryService _bookLibraryService;

    public BookLibraryController(BookLibraryService bookLibraryService)
    {
        _bookLibraryService = bookLibraryService;
    }

    //Visar upp böckerna med detaljerad information på sidan "Library.cshtml"
    public async Task<IActionResult> Index()
    {
        try
        {
            var books = await _bookLibraryService.GetBooksWithDetail();
            return View("Library", books);
        }
        catch (Exception ex)
        {
            ViewData["ApiError"] = ex.Message;
            return View("Library", new List<BookListDto>());
        }

    }
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> LibraryAdmin(int? editId, int? editAuthorId, int? editCategoryId)
    {
        var vm = new Booksearch.ViewModels.LibraryAdmin();

        try
        {
            vm.Books = await _bookLibraryService.GetBooksWithDetail();
            vm.Authors = await _bookLibraryService.GetAuthors();
            vm.Categories = await _bookLibraryService.GetCategories();

            vm.Form.Authors = vm.Authors;
            vm.Form.Categories = vm.Categories;

            if (editId.HasValue)
            {
                var book = await _bookLibraryService.GetBookById(editId.Value);

                vm.Form.Id = book.Id;
                vm.Form.Title = book.Title;
                vm.Form.Year = book.Year;
                vm.Form.AuthorId = book.AuthorId;
                vm.Form.CategoryId = book.CategoryId;
            }

            if (editAuthorId.HasValue)
            {
                var author = vm.Authors.FirstOrDefault(a => a.Id == editAuthorId.Value);
                if (author != null)
                {
                    vm.AuthorForm.Id = author.Id;
                    vm.AuthorForm.Name = author.Name;
                }
            }

            if (editCategoryId.HasValue)
            {
                var category = vm.Categories.FirstOrDefault(c => c.Id == editCategoryId.Value);
                if (category != null)
                {
                    vm.CategoryForm.Id = category.Id;
                    vm.CategoryForm.Name = category.Name;
                }
            }

            return View("LibraryAdmin", vm);
        }
        catch (Exception ex)
        {
            vm.ApiError = ex.Message;
            return View("LibraryAdmin", vm);
        }
    }

    //Skapa bok
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFromAdmin([Bind(Prefix = "Form")] Booksearch.ViewModels.BookFormVM vm) //chatGPT hjälp efter felsökning
    {
        try
        {
            // Skapar inte ny författare om jag skriver in en befintlig!
            if (!string.IsNullOrWhiteSpace(vm.NewAuthorName))
            {
                try
                {
                    var createdAuthor = await _bookLibraryService.CreateAuthor(vm.NewAuthorName);
                    vm.AuthorId = createdAuthor.Id;
                }
                catch
                {
                    // Om den redan finns: välj befintlig
                    var authors = await _bookLibraryService.GetAuthors();
                    var existing = authors.FirstOrDefault(a =>
                        a.Name.Equals(vm.NewAuthorName.Trim(), StringComparison.OrdinalIgnoreCase));

                    if (existing is null) throw; // något annat fel
                    vm.AuthorId = existing.Id;
                }
            }

            // Skapar inte ny kategori om jag skriver in en befintlig!
            if (!string.IsNullOrWhiteSpace(vm.NewCategoryName))
            {
                try
                {
                    var createdCategory = await _bookLibraryService.CreateCategory(vm.NewCategoryName);
                    vm.CategoryId = createdCategory.Id;
                }
                catch
                {
                    var categories = await _bookLibraryService.GetCategories();
                    var existing = categories.FirstOrDefault(c =>
                        c.Name.Equals(vm.NewCategoryName.Trim(), StringComparison.OrdinalIgnoreCase));

                    if (existing is null) throw;
                    vm.CategoryId = existing.Id;
                }
            }
            
            //skapa boken
            await _bookLibraryService.CreateBook(vm.Title, vm.Year, vm.AuthorId, vm.CategoryId);
            
            return RedirectToAction(nameof(LibraryAdmin));
        }
        catch (Exception ex)
        {
            //ladda om dropdownmenyerna
            vm.Authors = await _bookLibraryService.GetAuthors();
            vm.Categories = await _bookLibraryService.GetCategories();
            vm.ApiError = ex.Message;
            return View("LibraryAdmin", vm);
        }
    }
    
    //Ta bort bok
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBookFromAdmin(int id)
    {
        try
        {
            await _bookLibraryService.DeleteBook(id);
            return RedirectToAction(nameof(LibraryAdmin));
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    
    //Redigera
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([Bind(Prefix="Form")] BookFormVM vm)
    {
        try
        {
            // Ny author? Försök skapa annars använd befintlig
            if (!string.IsNullOrWhiteSpace(vm.NewAuthorName))
            {
                try
                {
                    var createdAuthor = await _bookLibraryService.CreateAuthor(vm.NewAuthorName);
                    vm.AuthorId = createdAuthor.Id;
                }
                catch
                {
                    var authors = await _bookLibraryService.GetAuthors();
                    var existing = authors.FirstOrDefault(a =>
                        a.Name.Equals(vm.NewAuthorName.Trim(), StringComparison.OrdinalIgnoreCase));

                    if (existing is null) throw;
                    vm.AuthorId = existing.Id;
                }
            }

            // Ny category? Försök skapa annars använd befintlig
            if (!string.IsNullOrWhiteSpace(vm.NewCategoryName))
            {
                try
                {
                    var createdCategory = await _bookLibraryService.CreateCategory(vm.NewCategoryName);
                    vm.CategoryId = createdCategory.Id;
                }
                catch
                {
                    var categories = await _bookLibraryService.GetCategories();
                    var existing = categories.FirstOrDefault(c =>
                        c.Name.Equals(vm.NewCategoryName.Trim(), StringComparison.OrdinalIgnoreCase));

                    if (existing is null) throw;
                    vm.CategoryId = existing.Id;
                }
            }

            await _bookLibraryService.UpdateBook(vm.Id!.Value, vm.Title, vm.Year, vm.AuthorId, vm.CategoryId);

            return RedirectToAction(nameof(LibraryAdmin)); // tillbaka till adminlistan
        }
        catch (Exception ex)
        {
            vm.Authors = await _bookLibraryService.GetAuthors();
            vm.Categories = await _bookLibraryService.GetCategories();
            vm.ApiError = ex.Message;
            return View("Edit", vm);
        }    
        
    }
    
    //skicka bok information från MVC till hyr API:
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rent(int bookId)
    {
        try
        {
            using var http = new HttpClient();

            var res = await http.PostAsJsonAsync(
                "https://webhook.site/66f599b5-1850-47ee-988e-1f49684c713f", //Byt senare till williams API!
                new { bookId = bookId }
            );

            TempData["Msg"] = res.IsSuccessStatusCode ? "Boken hyrdes!" : "Kunde inte hyra boken.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Msg"] = "Fel: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
    
    /* ta bort författare */
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAuthorFromAdmin(int id)
    {
        try
        {
            await _bookLibraryService.DeleteAuthor(id);
            return RedirectToAction(nameof(LibraryAdmin));
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    
    /* Ta bort kategori */
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategoryFromAdmin(int id)
    {
        try
        {
            await _bookLibraryService.DeleteCategory(id);
            return RedirectToAction(nameof(LibraryAdmin));
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> EditAuthor(int id)
    {
        try
        {
            var authors = await _bookLibraryService.GetAuthors();
            var author = authors.FirstOrDefault(a => a.Id == id);

            if (author == null)
                return NotFound();

            var vm = new AuthorUpdateDto
            {
                Id = author.Id,
                Name = author.Name
            };

            return View(vm);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAuthor([Bind(Prefix = "AuthorForm")] AuthorUpdateDto vm)
    {
        try
        {
            await _bookLibraryService.UpdateAuthor(vm.Id, vm);
            return RedirectToAction(nameof(LibraryAdmin));
        }
        catch (Exception ex)
        {
            var pageVm = new LibraryAdmin
            {
                Books = await _bookLibraryService.GetBooksWithDetail(),
                Authors = await _bookLibraryService.GetAuthors(),
                Categories = await _bookLibraryService.GetCategories(),
                AuthorForm = vm,
                ApiError = ex.Message
            };

            pageVm.Form.Authors = pageVm.Authors;
            pageVm.Form.Categories = pageVm.Categories;

            return View("LibraryAdmin", pageVm);
        }
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> EditCategory(int id)
    {
        try
        {
            var categories = await _bookLibraryService.GetCategories();
            var category = categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
                return NotFound();

            var vm = new CategoryUpdateDto
            {
                Id = category.Id,
                Name = category.Name
            };

            return View(vm);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory([Bind(Prefix = "CategoryForm")] CategoryUpdateDto vm)
    {
        try
        {
            await _bookLibraryService.UpdateCategory(vm.Id, vm);
            return RedirectToAction(nameof(LibraryAdmin));
        }
        catch (Exception ex)
        {
            var pageVm = new LibraryAdmin
            {
                Books = await _bookLibraryService.GetBooksWithDetail(),
                Authors = await _bookLibraryService.GetAuthors(),
                Categories = await _bookLibraryService.GetCategories(),
                CategoryForm = vm,
                ApiError = ex.Message
            };

            pageVm.Form.Authors = pageVm.Authors;
            pageVm.Form.Categories = pageVm.Categories;

            return View("LibraryAdmin", pageVm);
        }
    }
}