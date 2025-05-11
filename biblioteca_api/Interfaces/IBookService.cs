using biblioteca_api.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace biblioteca_api.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDTO>> GetAll();
        Task<BookDTO> GetById(int id);
        Task<BookDTO> Create(CreateBookDTO bookDto);
        Task Update(UpdateBookDTO bookDto);
        Task Delete(int id);

        Task<IEnumerable<BookDTO>> Search(string query);
        Task<IEnumerable<BookDTO>> GetAvailableBooks();
        Task<IEnumerable<BookDTO>> GetBooksByAuthor(string author);
        Task<IEnumerable<BookDTO>> GetBooksByPublicationYear(int year);

        Task<int> GetTotalBookCount();
        Task<int> GetAvailableCopiesCount(int bookId);
    }
}