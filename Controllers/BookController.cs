
using Bookstore.Data;
using Bookstore.Models;
using Microsoft.AspNetCore.Mvc;
using System;
public class BookController : Controller
{
   
    private readonly IWebHostEnvironment _env;


    public BookController( IWebHostEnvironment env)
    {
      
        _env = env;
    }


    public IActionResult Index()
    {
        var books = FakeData.Books;
        return View(books);
    }


    public IActionResult Create()
    {
        ViewBag.Categories = FakeData.Categories; 
        ViewBag.Authors = FakeData.Authors;     
        return View();
    }


    [HttpPost]
    public IActionResult Create(Book book, IFormFile imageFile)
    {
        if (imageFile != null)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            string path = Path.Combine(_env.WebRootPath, "images", fileName);
            using var stream = new FileStream(path, FileMode.Create);
            imageFile.CopyTo(stream);
            book.Image = fileName;
        }


        FakeData.Books.Add(book);
        return RedirectToAction("Index");
    }
}