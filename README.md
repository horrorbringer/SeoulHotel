# Windows Forms App — Git & Project Guide

![Build](https://img.shields.io/github/actions/workflow/status/OWNER/REPO/dotnet.yml?label=build)
![License](https://img.shields.io/github/license/OWNER/REPO)
![Issues](https://img.shields.io/github/issues/OWNER/REPO)
![PRs](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)

> Replace **OWNER/REPO** above with your repo path.

## Overview
A classic **Windows Forms** application built with **C#**. This README covers Git workflow, local setup, and day‑to‑day commands for both **Visual Studio** and **dotnet CLI**.

## Requirements
- Windows 10/11
- Visual Studio 2022 (17.x+) with **.NET desktop development**
- One of:
  - **.NET SDK 8.0+** (SDK‑style project), or
  - **.NET Framework 4.8** (legacy non‑SDK project)
- Git 2.40+

## Getting started

### 1) Clone
```bash
git clone https://github.com/OWNER/REPO.git
cd REPO
```

### 2) Open & restore (Visual Studio)
- Open the `*.sln` solution
- VS will auto‑restore NuGet packages
- Press **F5** to run

### 3) Build & run (CLI, SDK‑style)
```bash
dotnet restore
dotnet build
dotnet run --project src\MyWinFormsApp\MyWinFormsApp.csproj
```

## Repo layout (suggested)
```
REPO/
  .gitignore
  README.md
  MyWinFormsApp.sln
  src/
    MyWinFormsApp/
      MyWinFormsApp.csproj
      Program.cs
      Forms/
        MainForm.cs
        MainForm.Designer.cs
        MainForm.resx
      Properties/
        Resources.resx
        Settings.settings
  tests/
    MyWinFormsApp.Tests/
```

## Git workflow

### Branching
- `main` — stable, production‑ready
- `feat/<slug>` — features (e.g., `feat/customer-form`)
- `fix/<slug>` — bug fixes
- `chore/<slug>` — maintenance

### Commit messages
Use conventional prefixes:
```
feat: add CustomerForm with validation
fix: resolve null ref when loading settings
chore: upgrade NuGet packages
refactor: extract validation service
```

### Daily commands
```bash
# sync and branch
git pull --rebase origin main
git switch -c feat/customer-form

# work, stage & commit
git add .
git commit -m "feat: add CustomerForm with validation"

# push and open PR
git push -u origin feat/customer-form
```

### Tag releases
```bash
git tag -a v1.0.0 -m "WinForms v1.0"
git push origin v1.0.0
```

## NuGet packages
Restore is automatic in VS. For CLI:
```bash
dotnet restore
```

## Large files
Track heavy assets with **Git LFS**:
```bash
git lfs install
git lfs track "*.mp4" "*.zip" "*.psd"
git add .gitattributes
git commit -m "chore: track large assets with LFS"
```

## Troubleshooting

**Accidentally committed build output**
```bash
echo -e "bin/\nobj/" >> .gitignore
git rm -r --cached bin obj
git commit -m "chore: stop tracking build output"
```

**Designer merge conflicts**
1. Choose `ours` or `theirs` to get to a compilable state
2. Open the Form in VS Designer, make a no‑op change, save (Designer re‑orders safely)
3. Build to verify

**Package cache issues**
```bash
dotnet nuget locals all --clear
dotnet restore
```

## CI (GitHub Actions example)
Add `.github/workflows/dotnet.yml`:
```yaml
name: .NET
on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]
jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      - name: Restore
        run: dotnet restore
      - name: Build
        run: dotnet build --configuration Release --no-restore
```

## License
MIT © YOUR_NAME
