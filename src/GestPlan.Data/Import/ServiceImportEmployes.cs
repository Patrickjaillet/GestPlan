using System.Text;
using ClosedXML.Excel;
using GestPlan.Core.Entites;
using GestPlan.Data.Repositories;

namespace GestPlan.Data.Import;

/// <inheritdoc cref="IServiceImportEmployes"/>
public class ServiceImportEmployes(IUnitOfWorkFactory unitOfWorkFactory) : IServiceImportEmployes
{
    public IReadOnlyList<LigneImportEmploye> LireEtValiderCsv(Stream fluxCsv)
    {
        using var lecteur = new StreamReader(fluxCsv, Encoding.UTF8);
        var lignesBrutes = new List<string[]>();
        var premiereLigne = true;

        string? ligne;
        while ((ligne = lecteur.ReadLine()) is not null)
        {
            if (string.IsNullOrWhiteSpace(ligne))
            {
                continue;
            }

            if (premiereLigne)
            {
                premiereLigne = false;
                continue;
            }

            lignesBrutes.Add(ligne.Split(';'));
        }

        return ValiderLignes(lignesBrutes);
    }

    public IReadOnlyList<LigneImportEmploye> LireEtValiderExcel(Stream fluxExcel)
    {
        using var classeur = new XLWorkbook(fluxExcel);
        var feuille = classeur.Worksheets.First();

        var lignesBrutes = new List<string[]>();
        var premiereLigne = true;

        foreach (var ligne in feuille.RowsUsed())
        {
            var valeurs = new[]
            {
                ligne.Cell(1).GetString(),
                ligne.Cell(2).GetString(),
                ligne.Cell(3).GetString(),
                ligne.Cell(4).GetString(),
                ligne.Cell(5).GetString()
            };

            if (premiereLigne)
            {
                premiereLigne = false;
                continue;
            }

            lignesBrutes.Add(valeurs);
        }

        return ValiderLignes(lignesBrutes);
    }

    private static IReadOnlyList<LigneImportEmploye> ValiderLignes(IReadOnlyList<string[]> lignesBrutes)
    {
        var resultats = new List<LigneImportEmploye>();

        for (var i = 0; i < lignesBrutes.Count; i++)
        {
            var colonnes = lignesBrutes[i];
            var numeroLigne = i + 2; // +1 pour l'en-tête, +1 pour passer en 1-indexé

            var nom = ObtenirColonne(colonnes, 0);
            var prenom = ObtenirColonne(colonnes, 1);
            var email = ObtenirColonne(colonnes, 2);
            var telephone = ObtenirColonne(colonnes, 3);
            var nomSite = ObtenirColonne(colonnes, 4);

            var erreurs = new List<string>();

            if (string.IsNullOrWhiteSpace(nom))
            {
                erreurs.Add("Le nom est obligatoire.");
            }

            if (string.IsNullOrWhiteSpace(prenom))
            {
                erreurs.Add("Le prénom est obligatoire.");
            }

            if (string.IsNullOrWhiteSpace(nomSite))
            {
                erreurs.Add("Le site est obligatoire.");
            }

            if (!string.IsNullOrWhiteSpace(email) && !email.Contains('@'))
            {
                erreurs.Add("L'adresse e-mail est invalide.");
            }

            resultats.Add(new LigneImportEmploye
            {
                NumeroLigne = numeroLigne,
                Nom = nom,
                Prenom = prenom,
                Email = string.IsNullOrWhiteSpace(email) ? null : email,
                Telephone = string.IsNullOrWhiteSpace(telephone) ? null : telephone,
                NomSitePrincipal = nomSite,
                Erreurs = erreurs
            });
        }

        return resultats;
    }

    private static string ObtenirColonne(string[] colonnes, int index) =>
        index < colonnes.Length ? colonnes[index].Trim() : string.Empty;

    public async Task<int> ImporterAsync(IEnumerable<LigneImportEmploye> lignesValides, CancellationToken cancellationToken = default)
    {
        using var unitOfWork = unitOfWorkFactory.Creer();

        var sites = await unitOfWork.Sites.ObtenirTousAsync(cancellationToken);
        var sitesParNom = sites.ToDictionary(s => s.Nom, s => s, StringComparer.OrdinalIgnoreCase);

        var nombreImporte = 0;

        foreach (var ligne in lignesValides)
        {
            if (!ligne.EstValide || !sitesParNom.TryGetValue(ligne.NomSitePrincipal!, out var site))
            {
                continue;
            }

            var employe = new Employe
            {
                Nom = ligne.Nom!,
                Prenom = ligne.Prenom!,
                Email = ligne.Email,
                Telephone = ligne.Telephone,
                SitePrincipal = site
            };

            await unitOfWork.Employes.AjouterAsync(employe, cancellationToken);
            nombreImporte++;
        }

        if (nombreImporte > 0)
        {
            await unitOfWork.EnregistrerAsync(cancellationToken);
        }

        return nombreImporte;
    }
}
