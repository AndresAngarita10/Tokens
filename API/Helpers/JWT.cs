
namespace API.Helpers;

public class JWT
{
    public string Key { get; set; }// Llave secreta
    public string Issuer { get; set; } //quien genero el token
    public string Audience { get; set; }//A quien se le admitio el token
    public double DurationInMinutes { get; set; }//Cusantos minutos duracion de sesion
}
