using biblioteca_api.Data;
using biblioteca_api.Interfaces;
using biblioteca_api.Models.DTOs.Publisher;
using biblioteca_api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace biblioteca_api.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly LibraryContext _context;

        public PublisherService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PublisherDTO>> GetAll()
        {
            return await _context.Publishers
                .OrderBy(p => p.Name)
                .Select(p => new PublisherDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Address = p.Address,
                    Phone = p.Phone,
                    Email = p.Email,
                    FoundationDate = p.FoundationDate
                })
                .ToListAsync();
        }

        public async Task<PublisherDTO> GetById(int id)
        {
            var publisher = await _context.Publishers
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new KeyNotFoundException("Editorial no encontrada");

            return new PublisherDTO
            {
                Id = publisher.Id,
                Name = publisher.Name,
                Address = publisher.Address,
                Phone = publisher.Phone,
                Email = publisher.Email,
                FoundationDate = publisher.FoundationDate
            };
        }

        public async Task<PublisherDTO> Create(CreatePublisherDTO publisherDto)
        {
            var publisher = new Publisher
            {
                Name = publisherDto.Name,
                Address = publisherDto.Address,
                Phone = publisherDto.Phone,
                Email = publisherDto.Email,
                FoundationDate = publisherDto.FoundationDate
            };

            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();

            return new PublisherDTO
            {
                Id = publisher.Id,
                Name = publisher.Name,
                Address = publisher.Address,
                Phone = publisher.Phone,
                Email = publisher.Email,
                FoundationDate = publisher.FoundationDate
            };
        }

        public async Task Update(int id, CreatePublisherDTO publisherDto)
        {
            var existingPublisher = await _context.Publishers.FindAsync(id)
                ?? throw new KeyNotFoundException("Editorial no encontrada");

            existingPublisher.Name = publisherDto.Name;
            existingPublisher.Address = publisherDto.Address;
            existingPublisher.Phone = publisherDto.Phone;
            existingPublisher.Email = publisherDto.Email;
            existingPublisher.FoundationDate = publisherDto.FoundationDate;

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var publisher = await _context.Publishers
                .Include(p => p.Books)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (publisher == null) return;

            if (publisher.Books.Any())
                throw new InvalidOperationException("No se puede eliminar una editorial que tiene libros asociados");

            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PublisherDTO>> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return await GetAll();

            query = query.ToLower();
            return await _context.Publishers
                .Where(p => p.Name.ToLower().Contains(query) ||
                          p.Address.ToLower().Contains(query) ||
                          p.Email.ToLower().Contains(query))
                .Select(p => new PublisherDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Address = p.Address,
                    Phone = p.Phone,
                    Email = p.Email,
                    FoundationDate = p.FoundationDate
                })
                .ToListAsync();
        }
    }
}