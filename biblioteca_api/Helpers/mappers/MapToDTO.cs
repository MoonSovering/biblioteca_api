using biblioteca_api.Models.DTOs.Book;
using biblioteca_api.Models.DTOs.Author;
using biblioteca_api.Models.DTOs.Publisher;
using biblioteca_api.Models.DTOs.Supplier;
using biblioteca_api.Models.Entities;

namespace biblioteca_api.Helpers.Mappers
{
    public static class MapToDTOHelper
    {
        public static BookDTO MapToDTO(Book book)
        {
            if (book == null) return null;

            return new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.AuthorEntity?.Name ?? book.Author,
                AuthorEntity = book.AuthorEntity != null ? new AuthorSimpleDTO
                {
                    Id = book.AuthorEntity.Id,
                    Name = book.AuthorEntity.Name,
                    Nationality = book.AuthorEntity.Nationality
                } : null,
                Publisher = book.Publisher != null ? new PublisherSimpleDTO
                {
                    Id = book.Publisher.Id,
                    Name = book.Publisher.Name,
                    Country = book.Publisher.Address
                } : null,
                PublishedAt = book.PublishedAt,
                Price = book.Price,
                ISBN = book.ISBN,
                AvailableCopies = book.AvailableCopies,
                TotalCopies = book.TotalCopies,
                Supplier = book.Supplier != null ? new SupplierSimpleDTO
                {
                    Id = book.Supplier.Id,
                    Name = book.Supplier.CompanyName ?? book.Supplier.Username,
                    ContactEmail = book.Supplier.Email
                } : null
            };
        }
    }
}