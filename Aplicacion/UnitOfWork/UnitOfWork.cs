
using Aplicacion.Repository;
using Dominio.Entities;
using Dominio.Interfaces;
using Persistencia;

namespace Aplicacion.UnitOfWork;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly TokensContext context;
    /* private PaisRepository _paises; */
    private RolRepository _roles;
    private UserRepository _users;


    public UnitOfWork(TokensContext _context)
    {
        context = _context;
    }
 /*    public IPais Paises
    {
        get{
            if(_paises == null){
                _paises = new PaisRepository(context);
            }
            return _paises;
        }
    } */
    public IRol Rols
    {
        get{
            if(_roles == null){
                _roles = new RolRepository(context);
            }
            return _roles;
        }
    }
    
    public IUser Users
    {
        get{
            if(_users == null){
                _users = new UserRepository(context);
            }
            return _users;
        }
    }

    public void Dispose()
    {
        context.Dispose();
    }
    public async Task<int> SaveAsync()
    {
        return await context.SaveChangesAsync();
    }
}


