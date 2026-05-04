# Contributing to ArenaSync

## Getting Started

1. Fork the repository and clone your fork
2. Follow the setup instructions in [README.md](README.md)
3. Create a feature branch: `git checkout -b feature/my-feature`
4. Make your changes, add tests, and commit
5. Push to your fork and open a Pull Request

---

## Branch Naming

| Type | Format | Example |
|---|---|---|
| Feature | `feature/<short-description>` | `feature/vendor-export` |
| Bug fix | `fix/<short-description>` | `fix/event-delete-cascade` |
| Docs | `docs/<short-description>` | `docs/update-readme` |
| Refactor | `refactor/<short-description>` | `refactor/validation-service` |

---

## Commit Messages

Use the imperative mood and keep the subject line under 72 characters:

```
Add cascade delete for event child records
Fix SSR form binding for AM/PM select fields
Update DbSeeder to use dynamic venue ID lookups
```

---

## Pull Request Process

1. Ensure all tests pass: `dotnet test`
2. Ensure the app builds without warnings: `dotnet build`
3. Fill out the PR template completely
4. Request a review from at least one team member
5. Squash commits before merging (or use the GitHub "Squash and merge" button)

---

## Code Review Checklist

- [ ] Tests added or updated for all changed behavior
- [ ] No hardcoded IDs or magic numbers
- [ ] All database operations are async
- [ ] Exceptions are logged with context
- [ ] No sensitive data (passwords, connection strings) committed
- [ ] Razor pages use `[SupplyParameterFromForm]` on SSR form models
- [ ] New services are registered in `Program.cs`
- [ ] New entities have a migration

---

## Reporting Bugs

Use the Bug Report issue template. Include:
- Steps to reproduce
- Expected vs. actual behavior
- Relevant log output
- .NET version and OS

## Requesting Features

Use the Feature Request issue template. Describe the user story and the problem it solves.
