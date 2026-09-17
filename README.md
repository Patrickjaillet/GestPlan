# GestPlan

<p align="center">
  <img src="src/GestPlan.App/Assets/logo.png" alt="Logo GestPlan" width="128" height="128" />
</p>

Logiciel de planification des employés pour supérette (PME).

**Copyright © 2026 Sandefjord — Tous droits réservés**
Contact : sandefjord.development@proton.me
Site web : https://patrickjaillet.github.io/GestPlan

## Présentation

GestPlan est une application de bureau Windows destinée à la gestion du
planning des employés en supérette : création et suivi des créneaux de
travail, gestion des employés et de leurs contrats, suivi des congés et
absences, contrôle de conformité légale, calcul de la masse salariale et
exports (PDF, Excel).

## Fonctionnalités

> Le projet est en cours de développement. Les fonctionnalités ci-dessous
> seront complétées au fil des versions.

- Planning visuel par glisser-déposer (vue semaine, mois, par employé, par poste)
- Gestion des employés, contrats et compétences
- Gestion des congés et absences avec workflow de validation
- Moteur de règles de conformité légale (repos, durées maximales, pauses)
- Suivi des heures, coûts et masse salariale
- Exports PDF et Excel
- Gestion multi-site avec cloisonnement des droits par rôle

## Captures d'écran

*(Emplacement réservé — captures d'écran à venir dès que l'interface sera fonctionnelle.)*

## Technologies

- .NET 10 (LTS) / WPF
- [WPF-UI](https://github.com/lepoco/wpfui) (Fluent Design)
- CommunityToolkit.Mvvm
- Entity Framework Core (SQLite)
- QuestPDF, ClosedXML
- Serilog

## Installation

Voir [COMPILATION.md](COMPILATION.md) pour les instructions de compilation
depuis les sources. Un installeur Windows sera fourni à partir de la
version 1.0.0.

## Licence

Tous droits réservés — voir [LICENSE](LICENSE).
