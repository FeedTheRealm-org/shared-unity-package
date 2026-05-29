# Feed the Realm — Shared Package

Shared logic and UI components used across the game client and world editor Unity projects.

## Installation

Add the package to your Unity project via the Package Manager using the repository URL:

```
https://github.com/feedTheRealm-org/shared-unity-package.git
```

Or reference it directly in your project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.feed_the_realm.shared": "https://github.com/feedTheRealm-org/shared-unity-package.git"
  }
}
```

## Structure

```
.
├── Runtime/                  # Shared runtime code (included in builds)
│   ├── Core/                 # Core systems and base classes
│   ├── Data/                 # Shared data definitions
│   ├── DefaultMaterials/     # Shared default materials
│   ├── Errors/               # Shared error types
│   ├── Models/               # Shared domain models
│   ├── Shaders/              # Shared shaders
│   ├── UI/                   # Shared UI components
│   └── World/                # Shared world logic
└── Editor/                   # Editor-only utilities (not included in builds)
    ├── SceneReferenceDrawer.cs   # Custom property drawer
    └── FTRSharedScriptIcons.cs   # Custom script icons
```

## CI/CD

| Workflow | Description |
|---|---|
| `precommit-check.yaml` | Linting and formatting checks on pull requests |
| `git-leaks.yaml` | Scans for accidentally committed secrets |
