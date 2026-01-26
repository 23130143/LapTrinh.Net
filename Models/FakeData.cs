using Bookstore.Models;
namespace Bookstore.Data
{
    public static class FakeData
    {
        public static List<User> Users = new()
    {
        new User
        {
            Id = 1,
            Name = "Admin",
            Email = "admin@gmail.com",
            Password = "123"
        }
    };

        public static List<Category> Categories = new();
        public static List<Author> Authors = new();
        public static List<Book> Books = new();
    }
}
