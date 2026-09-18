using CommunityToolkit.Mvvm.ComponentModel;
using GestPlan.Core.Entites;
using GestPlan.Core.Enumerations;

namespace GestPlan.App.ViewModels;

/// <summary>
/// Représentation d'une <see cref="Absence"/> pour l'affichage et l'édition.
/// </summary>
public partial class AbsenceEditableViewModel : ObservableObject
{
    public int Id { get; init; }

    public int EmployeId { get; init; }

    public string NomEmploye { get; init; } = string.Empty;

    public int SiteId { get; init; }

    public int TypeAbsenceId { get; init; }

    public string NomTypeAbsence { get; init; } = string.Empty;

    [ObservableProperty]
    private DateTime _dateDebut = DateTime.Today;

    [ObservableProperty]
    private DateTime _dateFin = DateTime.Today;

    [ObservableProperty]
    private StatutAbsence _statut;

    [ObservableProperty]
    private string? _motif;

    public int NombreJours => (DateFin.Date - DateDebut.Date).Days + 1;

    public static AbsenceEditableViewModel DepuisEntite(Absence absence, string nomEmploye, string nomTypeAbsence) => new()
    {
        Id = absence.Id,
        EmployeId = absence.EmployeId,
        NomEmploye = nomEmploye,
        SiteId = absence.SiteId,
        TypeAbsenceId = absence.TypeAbsenceId,
        NomTypeAbsence = nomTypeAbsence,
        DateDebut = absence.DateDebut.ToDateTime(TimeOnly.MinValue),
        DateFin = absence.DateFin.ToDateTime(TimeOnly.MinValue),
        Statut = absence.Statut,
        Motif = absence.Motif
    };
}
