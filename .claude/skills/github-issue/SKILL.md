---
name: github-issue
description: Investigate, test, fix, or close a SparkplugNet GitHub issue. Use for issue follow-ups that ask for a GitHub comment, a release version field, or a commit linked to an issue or pull request.
---

# SparkplugNet GitHub issues

Use the issue URL or number in the request and verify it belongs to `SeppPenner/SparkplugNet`. Read the issue, its comments, and any linked reproduction or pull request. Use the connected GitHub tools, an authenticated `gh` session, or the authenticated GitHub REST API.

1. Reproduce the report through the relevant code path. For a fix, add a focused regression test when the existing tests miss the case. Verify the fix on the supported target frameworks. For broker-dependent reports, see the [HiveMQ test setup](../../../testing/sparkplug-tck/README.md).
2. When asked to commit, follow `CLAUDE.md` and put the full issue or pull request URL on a `Refs:` line in the commit body. When asked to push, verify the commit is reachable from the remote branch before citing it as published.
3. When asked to comment, write a short reply in the issue's language. Include the published commit link, one sentence on what changed, and a specific test result if useful. Use `no-ai-slop` if available. Check the issue for an equivalent comment before posting or retrying.
4. When asked to set the release version, update the issue's **Version** field with the supplied value and read it back. See [Setting the Version field](references/version-field.md).
5. When asked to close a fixed issue, verify the published commit and any requested comment that links to it. Close it as completed with `gh issue close ISSUE_URL --reason completed` or PATCH the issue with `{"state":"closed","state_reason":"completed"}`. Read the issue back and report its state, comment link, and any requested update still pending.

Respect the user's requested order and any conditions on GitHub changes.
