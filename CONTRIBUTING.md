# Contributing to The Great Reading Adventure

:tada: Thanks for your interest in contributing to The Great Reading Adventure! :fireworks:

Everyone is invited to contribute.

## If you use the software

- Help keep the [manual](http://manual.greatreadingadventure.com/en/latest/) up to date and accurate.
  - Not familiar with [Read the Docs](https://readthedocs.org/) or [reStructuredText](http://docutils.sourceforge.net/rst.html)? Contribute whatever you can via the [forum](http://forum.greatreadingadventure.com/) and we'll get it reformatted and added to the manual.
- Post helpful tips and resources to the [forum](http://forum.greatreadingadventure.com/).

## If you develop software

### Coding guidelines
- Fallback for any coding guideline questions is Microsoft's [C# Coding Conventions (C# Programming Guide)](https://msdn.microsoft.com/en-us/library/ff926074.aspx).
- Use `dotnet --verbose build` to check for additional compilation warnings.
- Wrap lines at 100 characters whenever possible.
- When wrapping, prefer putting operators at the start of wrapped lines, e.g.:
```
if(spot.IsPrettyCat
   && spot.IsGoodCat)
```
- Format code using the default Visual Studio code formatting (`^E^D`).
- Format any T-SQL with the [Poor Man's T-SQL Formatter](http://architectshack.com/PoorMansTSqlFormatter.ashx).
- Right-click (or `^.`) in C# files and *Remove Unnecessary Usings* or *Organize Usings* -> *Remove and Sort*
- Methods which are `async` should have the method name suffxed with "Async".
- Follow this guide for commit messages: [How to Write a Git Commit Message](http://chris.beams.io/posts/git-commit/) ([GitMagic](https://gitmagic.io/) should inform you if you violate these once you open a pull request on GitHub).

### Areas we need help
- Automated tests
- Unit tests
