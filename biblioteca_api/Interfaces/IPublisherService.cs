using biblioteca_api.Models.DTOs.Publisher;
namespace biblioteca_api.Interfaces
{
    public interface IPublisherService
    {
        Task<IEnumerable<PublisherDTO>> GetAll();
        Task<PublisherDTO> GetById(int id);
        Task<PublisherDTO> Create(CreatePublisherDTO publisherDto);
        Task Update(int id, CreatePublisherDTO publisherDto);
        Task Delete(int id);
        Task<IEnumerable<PublisherDTO>> Search(string query);
    }
}