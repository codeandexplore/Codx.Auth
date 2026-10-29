# Versioning

Codx.Auth uses **tag-driven Semantic Versioning** via [GitVersion](https://gitversion.net/). This document explains the versioning scheme, how releases are cut, and how the version is surfaced throughout the app, CI, and container images.

## Overview

- **Source of truth**: a git tag `vX.Y.Z` on `master` — the tag *is* the release.
- **Scheme**: [SemVer](https://semver.org/) `MAJOR.MINOR.PATCH`.
- **Tool**: GitVersion (via the `GitVersion.MsBuild` package), configured for `ContinuousDelivery`.
- **Branching**: `feature/*` → `development` → `master`.

You decide **which commit** to tag and **what number** to give it — GitVersion does not auto-bump on a tag.

## Branch → version mapping

| Branch | Version kind | Example |
|---|---|---|
| `development` | pre-release | `1.3.0-beta.5` |
| `master` (tagged `v1.3.0`) | release | `1.3.0` |
| `master` (untagged) | release + build metadata | `1.3.0+2` |
| `feature/*` | not released (CI build only) | — |

## How the version is surfaced

1. **Assembly** — `GitVersion.MsBuild` stamps `AssemblyVersion`/`InformationalVersion` at build time.
2. **`/health` endpoint** — returns the running version:
   ```
   GET /health  →  { "status": "Healthy", "version": "1.2.3" }
   ```
3. **Startup log** — prints `Codx.Auth version <x> starting`.
4. **Docker image tag** — `:<semver>` (immutable) and `:latest` (latest release).

## Releasing

1. Merge work through `development` → `master` as usual.
2. Tag the commit you want to release on `master`:
   ```bash
   git checkout master
   git pull
   git tag v1.2.3
   git push origin v1.2.3
   ```
3. The `Release` GitHub Actions workflow builds the image and pushes `:1.2.3` + `:latest` to `ghcr.io/codeandexplore/codx.auth/codx-auth`.

## Bumping the version

| Bump | When |
|---|---|
| **Patch** (`1.2.3` → `1.2.4`) | Bug fixes |
| **Minor** (`1.2.3` → `1.3.0`) | New, backward-compatible features |
| **Major** (`1.2.3` → `2.0.0`) | Breaking changes |

The bump is expressed by the tag you choose — there is no automatic increment step.

## CI/CD

| Workflow | Trigger | Action |
|---|---|---|
| `.github/workflows/build.yml` | push to `development`/`master`, pull request | restore → build → unit test (no image) |
| `.github/workflows/release.yml` | tag `v*` | build Docker image → push `:<semver>` + `:latest` |

## Configuration

- `GitVersion.yml` (repo root) — `mode: ContinuousDelivery`.
- `GitVersion.MsBuild` — package reference in `src/Codx.Auth/Codx.Auth.csproj`.
- `.dockerignore` — includes `.git` so GitVersion can compute the version during the Docker build.

## Related

- `RELEASING.md` (repo root) — release steps and consumer pinning guidance.
