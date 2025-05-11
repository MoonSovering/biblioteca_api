using biblioteca_api.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace biblioteca_api.Interfaces
{
    public interface ISupplierService
    {
        Task<Book> AddBook(int supplierId, Book book);
        Task UpdateBook(int supplierId, Book book);
        Task RemoveBook(int supplierId, int bookId);

        Task<IEnumerable<Book>> GetSuppliedBooks(int supplierId);
        Task<IEnumerable<Book>> GetAvailableSuppliedBooks(int supplierId);
        Task<IEnumerable<Loan>> GetBookLoans(int supplierId, int bookId);
        Task<IEnumerable<Loan>> GetAllSuppliedLoans(int supplierId);

        Task<int> GetSuppliedBooksCount(int supplierId);
        Task<decimal> CalculateSupplierRevenue(int supplierId, DateTime startDate, DateTime endDate);
        Task<Dictionary<string, int>> GetLoanStatsBySupplier(int supplierId);

        Task<bool> IsBookFromSupplier(int supplierId, int bookId);
    }
}