namespace GestPlan.Core.Entites;

/// <summary>
/// Un poste que l'employé est habilité à occuper (compétence). Un employé peut être
/// autorisé sur plusieurs postes (caisse, rayon, responsable...).
/// </summary>
public class EmployePoste
{
    public int EmployeId { get; set; }

    public Employe Employe { get; set; } = null!;

    public int PosteId { get; set; }

    public Poste Poste { get; set; } = null!;
}
