using biblioteca_api.Models.DTOs;
using biblioteca_api.Models.Entities;

namespace biblioteca_api.Helpers.Mappers
{
    public static class MapToDTOHelper
    {
        public static BookDTO MapToDTO(Book book)
        {
            if (book == null)
            {
                return null;
            }

            return new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                PublishedAt = book.PublishedAt,
                Price = book.Price,
                ISBN = book.ISBN,
                AvailableCopies = book.AvailableCopies,
                TotalCopies = book.TotalCopies,
                Supplier = book.Supplier != null ? new SupplierSimpleDTO
                {
                    Id = book.Supplier.Id,
                    Name = book.Supplier.CompanyName ?? book.Supplier.Username
                } : null
            };
        }
    }
}