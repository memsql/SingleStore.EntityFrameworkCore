## Release process

`EntityFrameworkCore.SingleStore` releases are automated through GitHub Actions. A new NuGet package is built and published automatically when a new version tag is pushed to the GitHub repository.

GitHub Releases are created manually after the package is published, because the release notes must be reviewed and added manually.

### Prerequisites

Before creating a release tag:

* Make sure the release changes are merged into the default branch.
* Make sure CI is passing.
* Update `Version.props` with the new package version.
* Make sure the version tag matches the version in `Version.props`.
* The version tag should use the `vX.Y.Z` format.

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

### Verifying the NuGet release

After the release workflow finishes successfully:

1. Check that the GitHub Actions workflow completed without errors.
2. Verify that the new package version is available on NuGet.
3. Optionally install the released package locally:

```bash
dotnet add package EntityFrameworkCore.SingleStore --version X.Y.Z
```

### Creating the GitHub Release

After the NuGet package is published, create the GitHub Release manually:

1. Open the repository's **Releases** page.
2. Create a new release for the pushed tag, for example `vX.Y.Z`.
3. Add the release title and release notes.
4. Publish the GitHub Release.

The GitHub Release is intentionally manual because it requires complete release notes.

### Failed releases

If the release workflow fails before publishing to NuGet, fix the issue and rerun the workflow or recreate the tag as needed.

If the package was already published to NuGet, do not reuse the same version number. NuGet package versions are immutable, so a fix must be released with a new version.
