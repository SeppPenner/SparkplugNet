# Setting the Version field

In the issue's right sidebar, find **Version** or open **Add field** and select **Version**. Enter the requested value in the text field or choose the matching option. GitHub saves the change automatically. Verify the displayed value after refreshing. [GitHub issue fields guide](https://docs.github.com/en/issues/tracking-your-work-with-issues/using-issues/adding-and-managing-issue-fields)

## API with GitHub CLI

`SeppPenner/SparkplugNet` is user-owned. Read existing values first. If **Version** is already set, use its `issue_field_id`. Otherwise find the exact field in the repository owner's personal issue fields while authenticated as the owner. Replace `ISSUE_NUMBER` with the requested issue number.

```sh
gh api repos/SeppPenner/SparkplugNet/issues/ISSUE_NUMBER/issue-field-values -H 'X-GitHub-Api-Version: 2026-03-10'
gh api user/issue-fields --paginate -H 'X-GitHub-Api-Version: 2026-03-10' --jq '.[] | select(.name == "Version")'
```

Save a JSON request in a temporary file. Replace `123` with the discovered field ID and `1.5.0` with the requested version. A single-select value must match an existing option by name.

```json
{"issue_field_values":[{"field_id":123,"value":"1.5.0"}]}
```

Send a POST with that one field to preserve the issue's other values. Read the issue field values again and verify **Version**. The response may expose a single-select value through `single_select_option.name`. [GitHub issue field values API](https://docs.github.com/en/rest/issues/issue-field-values)

```sh
gh api --method POST repos/SeppPenner/SparkplugNet/issues/ISSUE_NUMBER/issue-field-values -H 'X-GitHub-Api-Version: 2026-03-10' --input /path/to/version-field.json
gh api repos/SeppPenner/SparkplugNet/issues/ISSUE_NUMBER/issue-field-values -H 'X-GitHub-Api-Version: 2026-03-10'
```

## Permissions

Listing personal fields requires `read:user` on a classic token or OAuth session, or **Issue Fields (read)** on a fine-grained token. To update the issue, the account needs push access and a fine-grained token needs **Issues (write)** on the repository. For an authorized scope change to a stored `gh` session, use `gh auth refresh --hostname github.com --scopes read:user`. A repo-scoped token can comment and close an issue while field discovery returns `403`. [Personal issue fields API](https://docs.github.com/en/rest/users/issue-fields) · [GitHub CLI authentication](https://cli.github.com/manual/gh_auth_refresh)

If field discovery is blocked, use an authorized signed-in browser if available. If the field or option cannot be found, or access is still denied, report that the version is pending. Treat creating an account field or modifying its options as a separate change.
