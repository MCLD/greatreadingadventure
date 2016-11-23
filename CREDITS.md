# Credits

The Great Reading Adventure is open source software developed with the help of [organizations](#organizations), [individuals](#people), [services and software](#servicessoftware), and [other open-source software code and components](#source).

## <a name="organizations"></a>Organizations

- [Maricopa County Library District (MCLD)](http://www.mcldaz.org/)
- [Arizona State Library, Archives and Public Records](http://www.azlibrary.gov/)
- [Institute of Museum and Library Services](http://www.imls.gov/)
- [Maricopa County Education Service Agency (MCESA)](http://education.maricopa.gov/site/default.aspx?PageID=196)

## <a name="people"></a>People

- [Caris O'Malley](http://carisomalley.com/) &#9734;
- Brianna King
- [Dan Wilcox](https://github.com/iafb/)
- Danette Barton
- [Daniel Messer](http://cyberpunklibrarian.com/)
- [Harald Nagel](https://haraldnagel.com/)
- [Holly Brennan](https://www.linkedin.com/in/holly-brennan-76450270)
- [Ian Griffin](https://github.com/iangriffin/)
- [Justin Meiners](https://justinmeiners.github.io/)
- [Lucas Gonzalez](http://diffracted.com/)
- [Tara Carpenter](http://www.taracarpenter.com/)
- [Tony Apodaca](https://antonioramonapodaca.wordpress.com/)

## <a name="servicessoftware"></a>Services and software

- [AppHarbor](https://appharbor.com/) - free hosting tier that will run the GRA for testing
- [AppVeyor](http://www.appveyor.com/) - continuous integration, ensuring code check-ins don't break Windows builds
- [Discourse](https://www.discourse.org/) - powers [forum.greatreadingadventure.com](http://forums.greatreadingadventure.com)
- [dmarcian](https://dmarcian.com/) - helps keep email spam-free
- [GitHub](https://github.com/) - hosts the source code, issue tracker, Wiki, [www.greatreadingadventure.com](http://www.greatreadingadventure.com/) and more
- [GitMagic](https://gitmagic.io/) - enforces revision control guidelines
- [Glitch public domain game art](https://www.glitchthegame.com/public-domain-game-art/) - free fantastic art which can be seen in the default banner and avatars
- [Mailtrap](https://mailtrap.io/) - painless email testing during development
- [MyKnowledgeMap](https://www.myknowledgemap.com/) - creators and maintainers of [OpenBadges.me](https://www.openbadges.me/), our integrated badge maker
- [Open Library](https://openlibrary.org/) - book cover images, title, and author lookups for challenges
- [Read The Docs](https://readthedocs.org/) - powers [the online manual](http://manual.greatreadingadventure.com)
- [Slack](https://slack.com/) - team communications
- [Travis CI](https://travis-ci.org/) - continuous integration, ensuring code check-ins don't break Linux and macOS builds

## <a name="source"></a>Open-source software and components

The Great Reading Adventure uses open source components. You can find information about those components along with links to their source code and license information below. We are grateful to these developers for their contributions to the open source community, without them The Great Reading Adventure wouldn't exist!

### .NET Core
- [.NET Core](https://www.microsoft.com/net/core) is a general purpose development platform maintained by Microsoft and the .NET community on GitHub. It is cross-platform, supporting Windows, macOS and Linux, and can be used in device, cloud, and embedded/IoT scenarios.
- Source on GitHub: [dotnet/core](https://github.com/dotnet/core) - [MIT License](https://github.com/dotnet/core/blob/master/LICENSE)

### AutoMapper
- [AutoMapper](http://automapper.org/): A convention-based object-object mapper.
- Source on GitHub: [automapper/automapper](https://github.com/AutoMapper/AutoMapper) - [MIT License](https://github.com/AutoMapper/AutoMapper/blob/master/LICENSE.txt)

### Entity Framework Core
- [Entity Framework Core](https://github.com/aspnet/EntityFramework) is an object-relational mapper (O/RM) that enables .NET developers to work with a database using .NET objects. It eliminates the need for most of the data-access code that developers usually need to write.
- Source on GitHub: [aspnet/EntityFramework](https://github.com/aspnet/EntityFramework) - [Apache License, Version 2.0](https://github.com/aspnet/EntityFramework/blob/dev/LICENSE.txt)

### PasswordHasher
- Securely hash passwords for storage in the database and for valiating logins.
- Adapted code from the [ASP.NET Identity PasswordHasher class](https://github.com/aspnet/Identity/blob/dev/src/Microsoft.AspNetCore.Identity/PasswordHasher.cs)
- Source on GitHub: [aspnet/Identity](https://github.com/aspnet/Identity) - [Apache License, Version 2.0](https://github.com/aspnet/Identity/blob/dev/LICENSE.txt)

### Session serialization/deserialization code snippet
- The session serialization/deserialization code allows complex objects to be stored in the ASP.NET Session by serializing them to [JSON](http://www.json.org/).
- From the blog post [Using Sessions and HttpContext in ASP.NET Core and MVC Core](http://benjii.me/2016/07/using-sessions-and-httpcontext-in-aspnetcore-and-mvc-core/) by [Ben Cull](https://bencull.com/)
