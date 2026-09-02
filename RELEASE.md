## Release process

`EntityFrameworkCore.SingleStore` releases are automated through GitHub Actions. A new NuGet package is built and published automatically when a new version tag is pushed to the GitHub repository.

A draft GitHub Release is also created automatically. The release remains a draft because the release notes must be reviewed and completed manually before publishing.

### Prerequisites

Before creating a release tag:

* Make sure the release changes are merged into the default branch.
* Make sure CI is passing.
* Update `Version.props` with the new package version.
* Make sure the version tag matches the version in `Version.props`.
* The version tag must use the `vX.Y.Z` format.

For example, if `Version.props` contains version `9.0.1`, the release tag should be `v9.0.1`.

### Creating a release

From the default branch, run:

```bash
git checkout master
git pull origin master
git tag vX.Y.Z
git push origin vX.Y.Z
```

Replace `X.Y.Z` with the version being released.

After the tag is pushed, GitHub Actions will automatically:

1. Run the test workflows.
2. Build the EF Core provider package.
3. Pack the NuGet package.
4. Publish the `.nupkg` package to NuGet.
5. Create a draft GitHub Release for the pushed tag.

### Verifying the NuGet release

After the release workflow finishes successfully:

1. Check that the GitHub Actions workflow completed without errors.
2. Verify that the new package version is available on NuGet.
3. Optionally install the released package locally:

```bash
dotnet add package EntityFrameworkCore.SingleStore --version X.Y.Z
```

### Publishing the GitHub Release

After the workflow creates the draft GitHub Release:

1. Open the repository's Releases page.
2. Open the draft release for the pushed tag, for example `vX.Y.Z`.
3. Review and complete the release notes.
4. Publish the GitHub Release.

The GitHub Release is intentionally kept as a draft because it requires complete release notes before publishing.

### Failed releases

If the release workflow fails before publishing to NuGet, fix the issue and rerun the workflow or recreate the tag as needed.

If the package was already published to NuGet, do not reuse the same version number. NuGet package versions are immutable, so a fix must be released with a new version.

## Driver-Server Version Compatibility Matrix

After each release, add a row for the new version rather than copying an older row's engine list. Tags through `v8.0.0` used a pinned engine matrix in CircleCI (through `v7.0.1`) and then GitHub Actions (`v8.0.0`). From `v8.0.3`, CI has no pinned engine matrix (dev image plus Helios); take the list from the [EOL policy](https://docs.singlestore.com/db/v9.1/support/singlestore-software-end-of-life-eol-policy/) as of the new tag's date, plus any engine RC that existed by that date.

| Driver Version | Release date | Supported engine versions |
| -------------- | ------------ | ------------------------- |
| 9.0.0          | 2026-04-21   | 8.7, 8.9, 9.0, 9.1 RC     |
| 8.0.3          | 2026-02-02   | 8.7, 8.9, 9.0             |
| 8.0.0          | 2025-07-22   | 8.5, 8.7                  |
| 7.0.1          | 2025-03-11   | 8.1, 8.5, 8.7             |
| 7.0.0          | 2024-11-18   | 8.1, 8.5, 8.7             |
| 6.0.2          | 2024-04-18   | 8.1, 8.5                  |
