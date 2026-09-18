# Journal des modifications

Toutes les modifications notables apportées à ce projet sont documentées dans ce fichier.

Le format s'appuie sur [Keep a Changelog](https://keepachangelog.com/fr/1.1.0/),
et ce projet adhère au [Versionnage Sémantique](https://semver.org/lang/fr/) (SemVer).

## [Non publié]

### Ajouté

- Logo et icône de l'application (`Assets/logo.svg`, `Assets/logo.png`, `Assets/app.ico`), intégrés à l'exécutable et à la fenêtre principale.
- Modèle de données complet : entités `Site`, `Utilisateur`, `Employe`, `Poste`, `Contrat` et `JournalAudit`.
- Contexte de base de données EF Core (SQLite) avec journalisation d'audit automatique (qui, quoi, quand, avant/après) sur toute création, modification ou suppression.
- Première migration de base de données et mécanisme de mise à jour automatique et transparente au démarrage, précédé d'une sauvegarde horodatée du fichier de base existant.
- Pattern Repository et Unit of Work pour l'accès aux données.
- Fenêtre principale avec navigation (Planning, Employés, Congés, Rapports, Sites, Paramètres, À propos) et thème clair/sombre suivant automatiquement le thème Windows.
- Mécanisme minimal de chargement des chaînes d'interface depuis les ressources d'internationalisation.
- Gestion multi-site : extension du modèle `Site` (adresse, horaires d'ouverture, fuseau horaire), sélecteur de site global persisté entre sessions, rattachement des employés à un site principal et à des sites secondaires, paramètres de conformité par site.
- Écran de gestion des sites (création, modification, désactivation/réactivation).
- Authentification : écran de connexion au démarrage, création guidée du premier compte administrateur, hachage sécurisé des mots de passe (BCrypt), politique de mot de passe (longueur, complexité), verrouillage temporaire après plusieurs échecs, déconnexion automatique après inactivité.
- Contrôle d'accès par rôle (Admin, Manager, Consultation) et cloisonnement des données par site assigné.
- Écran de gestion des comptes utilisateurs (réservé aux administrateurs).
- Journalisation des connexions et déconnexions.
- Gestion complète des employés : fiche identité (coordonnées, photo), contrats (type, quotité, heures, taux horaire, historique des avenants/renouvellements), compétences/postes autorisés, indisponibilités récurrentes, archivage.
- Écran de gestion des postes de travail par site (caisse, rayon, responsable...).
- Import en masse d'employés depuis un fichier CSV ou Excel, avec validation des données avant import.
- Recherche et filtres avancés sur la liste des employés (nom/prénom, site, poste, statut).
- Planning visuel : grille interactive (vues semaine, mois, par employé, par poste) avec création de créneaux par glisser-déposer et redimensionnement par poignées, duplication de semaine, statuts de créneau (brouillon, planifié, confirmé, annulé), code couleur par poste, détection automatique des conflits (double affectation, indisponibilité), verrouillage des créneaux passés, vue consolidée multi-site, annulation/rétablissement des modifications en session, mode brouillon/publié.
- Localisation forcée en français (formats d'heure et de date) sur l'ensemble de l'interface.
- Congés et absences : types d'absence paramétrables, demande et workflow de validation (validé/refusé avec traçabilité), calcul et affichage du solde de congés par employé et par année, acquisition automatique des droits selon des règles paramétrables par site, blocage et alerte en cas de chevauchement avec le planning, vue calendrier consolidée des absences validées, export PDF du solde de congés.

## [0.1.0] - 2026-09-17

### Ajouté

- Initialisation du dépôt Git et de la structure du projet.
- Solution `GestPlan.sln` (.NET 10 LTS) composée de quatre projets : `GestPlan.App` (WPF), `GestPlan.Core`, `GestPlan.Data` et `GestPlan.Tests`.
- Intégration de WPF-UI (Fluent Design) pour l'interface graphique.
- Intégration de CommunityToolkit.Mvvm pour l'implémentation du pattern MVVM.
- Mise en place de l'injection de dépendances via l'hôte générique (`Microsoft.Extensions.Hosting`).
- Configuration de Serilog (journalisation fichier avec rotation quotidienne, rétention 31 jours, sortie de débogage).
- Fenêtre principale minimale (`FluentWindow`) confirmant le démarrage de l'application.
- Fichier de ressources d'internationalisation `fr-FR.json` (français par défaut).
- Squelettes de documentation : `README.md`, `COMPILATION.md`, `LICENSE`.
- Définition de la version initiale du logiciel : `0.1.0`.

[Non publié]: https://patrickjaillet.github.io/GestPlan
[0.1.0]: https://patrickjaillet.github.io/GestPlan
