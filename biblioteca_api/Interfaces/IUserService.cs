﻿using biblioteca_api.Models.Entities;
using System.Security.Authentication;

namespace biblioteca_api.Interfaces
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(int id);
        Task<User> Create(User user, string password);
        Task Update(User user, string password = null);
        Task Delete(int id);
        Task AssignRole(int userId, int roleId);
        Task<IEnumerable<string>> GetUserRoles(int userId);
    }
}