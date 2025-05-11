using biblioteca_api.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace biblioteca_api.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAll();
        Task<Book> GetById(int id);
        Task<Book> Create(Book book);
        Task Update(Book book);
        Task Delete(int id);

        Task<IEnumerable<Book>> Search(string query);
        Task<IEnumerable<Book>> GetAvailableBooks();
        Task<IEnumerable<Book>> GetBooksByAuthor(string author);
        Task<IEnumerable<Book>> GetBooksByPublicationYear(int year);

        Task<int> GetTotalBookCount();
        Task<int> GetAvailableCopiesCount(int bookId);
    }
}