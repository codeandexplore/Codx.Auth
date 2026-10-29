# Versioning & Releasing Codx.Auth

Codx.Auth uses tag-driven **Semantic Versioning** via GitVersion. Docker images are built and pushed to GitHub Container Registry (GHCR) by **GitHub Actions** — no manual build step.

## Image tags

Registry: `ghcr.io/codeandexplore/codx.auth/codx-auth`

| Tag | Meaning |
|---|---|
| `:<semver>` (e.g. `:1.2.3`) | Immutable release — **pin to this** |
| `:latest` | Latest release (moving tag) |

## Releasing

1. Merge work to `development` (integration) and `master` (production) as usual.
2. Tag the commit you want to release on `master`:

   ```bash
   git checkout master
   git pull
   git tag v1.2.3
   git push origin v1.2.3
   ```

3. GitHub Actions builds the image and pushes `:1.2.3` + `:latest` to GHCR.

The version comes from the tag — GitVersion reads `v1.2.3`, stamps `1.2.3` into the assembly (reported by `/health`) and the image tag.

## Health / version

The running app reports its version:

```
GET /health  →  { "status": "Healthy", "version": "1.2.3" }
```

The startup log also prints `Codx.Auth version <x> starting`.

## For consumers

- **Pin** `:<semver>` (e.g. `ghcr.io/codeandexplore/codx.auth/codx-auth:1.2.3`) so your deployment only changes when you explicitly bump it.
- `:latest` always equals the latest release — convenient, but not rollback-safe.
