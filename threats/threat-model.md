# Simple Threat Model Demo: C# API

**What are we building?**  
A tiny ASP.NET Core API that accepts user input.

**What can go wrong?**  
An attacker could send unexpected input that the API does not handle safely (for example, very long strings or malformed data that could cause problems downstream).

**What are we going to do about it?**  
The CI build will fail if the code contains an obviously unsafe pattern that concatenates user input into a query-like string.

## Single Threat

| Threat | What it is | How the build mitigates it |
|--------|------------|----------------------------|
| Unsafe input handling | Code that directly concatenates external input into a command or query string | The GitHub Actions workflow searches for the unsafe pattern and fails the build if it is found |

This is intentionally minimal so the principle is clear:  
**identify a threat → add an automated check that blocks it.**
