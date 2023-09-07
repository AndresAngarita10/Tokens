

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Dtos;
using API.Helpers;
using Dominio.Entities;
using Dominio.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class UserService : IUserService
{
    public readonly JWT _jwt;
    public readonly IUnitOfWork _unitOfWork;
    public readonly IPasswordHasher<User> _passwordHasher;

    public UserService(IUnitOfWork unitOfWork, IOptions<JWT> jwt, IPasswordHasher<User> passwordHasher)
    {
        _jwt = jwt.Value;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> RegisterAsync(RegisterDto registerDto)
    {
        var usuario = new User
        {
            UserEmail = registerDto.Email,
            UserName = registerDto.Username
        };
        usuario.Password = _passwordHasher.HashPassword(usuario, registerDto.Password);
        var usuarioExiste = _unitOfWork.Users
                    .Find(u => u.UserName.ToLower() == registerDto.Username.ToLower())
                    .FirstOrDefault();
        if (usuarioExiste == null)
        {
           /*  var rolPredeterminado = _unitOfWork,Rols
                    .Find(u => u.Name_Rol == AuthorizationServiceCollectionExtensions.Rol_PorDefecto.ToString())
                    .First(); */
            try
            {
                //usuario.Rols.Add(rolPredeterminado);
                _unitOfWork.Users.Add(usuario);
                await _unitOfWork.SaveAsync();
                return $"El usuario {registerDto.Username} has sido registrado existosamente";
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return $"Error: {message}";
            }
        }
        else 
        {
            return $"El usuario con {registerDto.Username} Ya se encuentra registrado";
        }
    }

    public async Task<string> AddRoleAsync(AddRoleDto model)
    {
        var usuario = await _unitOfWork.Users
                        .GetByUserNameAsync(model.Username);
        if(usuario == null)
        {
            return $"No eciste algun usurio registrado con la cuente, olvido algun caracter?{model.Username}";

        }
        var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.Password, model.Password );

        if(resultado == PasswordVerificationResult.Success)
        {
            var rolExiste = _unitOfWork.Rols
                            .Find(rol => rol.Nombre.ToLower() == model.Rol.ToLower()) 
                            .FirstOrDefault();
            
            if(rolExiste != null)
            {
                var usuarioTienerol = usuario.Rols.Any(u => u.Id == rolExiste.Id);

                if(usuarioTienerol == false)
                {
                    usuario.Rols.Add(rolExiste);
                    _unitOfWork.Users.Update(usuario)    ;
                    await _unitOfWork.SaveAsync();
                }

                return $"Rol {model.Rol} agregado a la cuneta {model.Username} de forma exitosa";
            }
            return $"Rol {model.Rol} no encontrado";
        }
        return $"Credenciales incorrecta para el usuario: {model.Username}";
    }

    public async Task<DatosUsuarioDto> GetTokenAsync(LoginDto model)
    {
        DatosUsuarioDto datosUsuarioDto = new DatosUsuarioDto();
        var usuario = await _unitOfWork.Users
                        .GetByUserNameAsync(model.UserName);
        
        if(usuario == null)
        {
            datosUsuarioDto.EsAutenticado = false;
            datosUsuarioDto.Mensaje = $"No existe ningun usuario llamado: {model.UserName}";
            return datosUsuarioDto;
        }

        var result =_passwordHasher.VerifyHashedPassword(usuario, usuario.Password, model.Password);
        if(result == PasswordVerificationResult.Success)
        {
            datosUsuarioDto.Mensaje = "OK";
            datosUsuarioDto.EsAutenticado = true;
            if(usuario != null)
            {
                JwtSecurityToken jwtSecurityToken = CreateJwtToken(usuario);
                datosUsuarioDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                datosUsuarioDto.UserName = usuario.UserName;
                datosUsuarioDto.Email = usuario.UserEmail;
                datosUsuarioDto.Roles = usuario.Rols
                                        .Select(p => p.Nombre).ToList();
                return datosUsuarioDto;
            }else 
            {
                datosUsuarioDto.EsAutenticado = false;
                datosUsuarioDto.Mensaje = $"Credenciales incorrectas para el usuario {usuario.UserName}";
                return datosUsuarioDto;
            }
        }
        datosUsuarioDto.EsAutenticado = false;
        datosUsuarioDto.Mensaje = $"Credenciales incorrectas para el usuario {usuario.UserName}";
        return datosUsuarioDto;
    }

    private JwtSecurityToken CreateJwtToken(User usuario)
    {
        if(usuario == null)
        {
            throw new ArgumentNullException(nameof(usuario), "El usuario no puede ser nulo");
        }

        var roles = usuario.Rols;
        var roleClaims = new List<Claim>();
        foreach (var role in roles)
        {
            roleClaims.Add(new Claim("roles", role.Nombre));
        }

        var claims = new []
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("uid", usuario.Id.ToString())
        } 
        .Union(roleClaims);

        if(string.IsNullOrEmpty(_jwt.Key) || string.IsNullOrEmpty(_jwt.Issuer) || string.IsNullOrEmpty(_jwt.Audience))
        {
            throw new ArgumentNullException("La configuracion de l jwt es nula o vacia");
        }

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));

        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

        var JwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
            signingCredentials: signingCredentials
        );
        return JwtSecurityToken;
    }
}
