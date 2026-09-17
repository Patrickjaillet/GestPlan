# Compilation de GestPlan

## Prérequis

- Windows 10/11
- [SDK .NET 10 (LTS)](https://dotnet.microsoft.com/download/dotnet/10.0)
- Git

## Structure de la solution

```
GestPlan.sln
├── src/
│   ├── GestPlan.App/     Application WPF (interface utilisateur, point d'entrée)
│   ├── GestPlan.Core/    Logique métier, entités, règles de gestion
│   └── GestPlan.Data/    Accès aux données (EF Core, SQLite, repositories)
└── tests/
    └── GestPlan.Tests/   Tests unitaires et d'intégration (xUnit)
```

## Restauration des dépendances

```powershell
dotnet restore GestPlan.sln
```

## Compilation

```powershell
dotnet build GestPlan.sln
```

## Exécution

```powershell
dotnet run --project src/GestPlan.App/GestPlan.App.csproj
```

## Tests

```powershell
dotnet test GestPlan.sln
```

## Génération de l'installeur

*(À documenter à partir de la Phase 14 — packaging et distribution.)*

## Versionnage

Le numéro de version suit strictement [SemVer](https://semver.org/lang/fr/)
(`MAJOR.MINOR.PATCH`) et est défini dans [Directory.Build.props](Directory.Build.props)
via la propriété `VersionPrefix`.
