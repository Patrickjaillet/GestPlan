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
- Fenêtre principale avec navigation (Planning, Employés, Congés, Rapports, Paramètres, À propos) et thème clair/sombre suivant automatiquement le thème Windows.
- Mécanisme minimal de chargement des chaînes d'interface depuis les ressources d'internationalisation.

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
