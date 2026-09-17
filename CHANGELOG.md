# Journal des modifications

Toutes les modifications notables apportées à ce projet sont documentées dans ce fichier.

Le format s'appuie sur [Keep a Changelog](https://keepachangelog.com/fr/1.1.0/),
et ce projet adhère au [Versionnage Sémantique](https://semver.org/lang/fr/) (SemVer).

## [Non publié]

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
