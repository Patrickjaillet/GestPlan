namespace GestPlan.Core.Enumerations;

/// <summary>
/// Rôle d'un utilisateur, déterminant son périmètre d'accès. L'ordre des valeurs est
/// significatif : du plus privilégié (<see cref="Admin"/>) au moins privilégié
/// (<see cref="Consultation"/>), pour permettre les comparaisons de type « rôle minimum requis ».
/// </summary>
public enum RoleUtilisateur
{
    Admin,
    Manager,
    Consultation
}
