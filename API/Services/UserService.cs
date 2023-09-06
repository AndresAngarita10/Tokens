

using API.Helpers;
using Dominio.Entities;
using Dominio.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace API.Services;

public class UserService : IUserService
{
    public readonly JWT _jwt;
    public readonly IUnitOfWork _unitOfWork;
    public readonly IPasswordHasher<User> _passwordHasher;
}
