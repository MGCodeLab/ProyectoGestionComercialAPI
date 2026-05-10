namespace Application.Interfaces;

public interface IParametroSistemaValidatorService
{
    Task<bool> IsClaveUnique(string clave, CancellationToken cancellationToken);
    Task<bool> IsClaveUniqueExcept(int parametroId, string clave, CancellationToken cancellationToken);
}
