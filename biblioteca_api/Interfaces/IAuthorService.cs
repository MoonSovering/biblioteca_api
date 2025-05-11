using biblioteca_api.Models.DTOs.Author;

namespace biblioteca_api.Interfaces
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorDTO>> GetAll();
        Task<AuthorDTO> GetById(int id);
        Task<AuthorDTO> Create(CreateAuthorDTO authorDto);
        Task Update(int id, CreateAuthorDTO authorDto);
        Task Delete(int id);
        Task<IEnumerable<AuthorDTO>> Search(string query);
    }
}