# ADR-004: Tag-Driven Semantic Versioning with GitVersion

**Status**: Accepted
**Date**: 2026-10-28

## Decision

Codx.Auth is versioned using **tag-driven Semantic Versioning (SemVer)**. A git tag `vX.Y.Z` on `master` is a release; [GitVersion](https://gitversion.net/) (via the `GitVersion.MsBuild` package) reads the tag and stamps the version into the assembly and the Docker image tag. The running version is exposed through the `/health` endpoint and a startup log line. Docker images are built and pushed to GHCR automatically by GitHub Actions on release tags.

## Context

Codx.Auth's Docker image was previously built and pushed manually via a `.bat` file, always tagged `:latest`. This had three problems:

1. **No versioning** — consumers pulling `:latest` could not pin to a known-good version or roll back.
2. **Not reproducible** — the build depended on a developer's machine and its GHCR credentials.
3. **No runtime visibility** — the app could not report its own version, so "what's running" was unknowable.

### Versioning tool — alternatives considered

- **GitVersion.MsBuild (chosen)** — a build-time package that derives the version from git tags and history. Branch-aware (`development` → pre-release, `master` → release).
- **MinVer** — tag-driven and minimal, but not branch-aware; rejected because `development` and `master` need distinct versions.
- **GitVersion CLI / GitHub Action only** — computes the version in CI without stamping the assembly; rejected because the app should report its version via `/health`.
- **CI-derived tag (no .NET package)** — version lives only in the image tag; rejected because the running app needs the version in `/health`.

### Release cadence — alternatives considered

- **Tag-driven releases (chosen)** — you decide which commit and what number; consumers get deliberate, tested releases.
- **Every-merge release** — every merge auto-bumps and publishes; rejected (flood of versions, unclear SemVer meaning).

## Consequences

- **Positive:** Consumers can pin `:1.2.3` and roll back independently of `:latest`.
- **Positive:** Releases are reproducible — GitHub Actions builds and pushes with the built-in `GITHUB_TOKEN` (no laptop secret).
- **Positive:** `/health` reports the version, so operators can identify the running build.
- **Negative:** Requires tag discipline — a release only happens when someone tags `master`.
- **Negative:** `.dockerignore` must include `.git` so GitVersion can compute the version inside the Docker build (slightly larger build context).
- **Neutral:** `GitVersion.MsBuild` is pinned to a specific version (`6.0.2`) and can be bumped independently.

## Related

- `docs/versioning.md` — the versioning instruction/guide.
- `RELEASING.md` (repo root) — release steps and consumer guidance.
