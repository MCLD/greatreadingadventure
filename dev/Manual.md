### Introduction

The source for the Great Reading Adventure manual is located in the [`docs/`](https://github.com/MCLD/greatreadingadventure/tree/master/docs) directory of the repository.

The manual is formatted in [reStructuredText](http://docutils.sourceforge.net/rst.html) and [CommonMark](http://commonmark.org/) (a fork of [Markdown](http://daringfireball.net/projects/markdown/)). The Python tool [Sphinx](http://sphinx-doc.org/) can be used to build the documentation and (via GitHub integration) [Read The Docs](https://readthedocs.org/) automatically rebuilds the documentation upon commit and hosts it at [manual.greatreadingadventure.com](http://manual.greatreadingadventure.com/).

### Contributing

Anyone is welcome to contribute to the manual!

#### Style

1. Please ensure filenames are all lowercase and only contain letters and dash characters (-).
2. The easiest way to contribute is to write text in [Markdown](http://daringfireball.net/projects/markdown/syntax), however there are some things (like callout boxes) which can only be done with reStructuredText.
3. The [Sphinx Style Guide](http://documentation-style-guide-sphinx.readthedocs.org/en/latest/style-guide.html) provides in-depth style information for using reStructuredText.
4. Please ensure images are in a folder named `_static` for the section of the manual that they apply.

#### Forking and contributing

The best approach to editing the manual is to follow the [Forking Projects](https://guides.github.com/activities/forking/) guidance provided by GitHub using [GitHub Desktop](https://desktop.github.com/) or [GitKraken](https://www.gitkraken.com/) on your computer to manage the project files. By making your own fork of the project you are free to edit the documentation and then submit a pull request when you are ready for your edits to make it back into the main project.

#### Editing the files on your computer

Once you have your local copy, you can edit the files using the editing tool of your choice. The main index (`index.rst`) is formatted in reStructuredText. For simplicity, other files in the repository can be formatted in CommonMark/Markdown.

#### Pushing updates back to the project

Once your edits are complete,

1. [Commit your changes to your local repository and sync your local repository back to GitHub](https://help.github.com/desktop/guides/contributing/committing-and-reviewing-changes-to-your-project/)
2. [Send a pull request](https://help.github.com/desktop/guides/contributing/sending-a-pull-request/) to inform us that you want to incorporate your changes back into the main project.
3. You can [delete your fork](https://help.github.com/articles/deleting-a-repository/) if you do not intend to use it again or [keep it updated](http://stackoverflow.com/questions/20984802/how-can-i-keep-my-fork-in-sync-without-adding-a-separate-remote/21131381#21131381) if you wish to continue submitting changes.

### Building

If you wish to build the manual locally (to generate HTML or PDF yourself), you must install Python and Sphinx on your computer. This is probably unnecessary since manual updates are reflected in the [online manual](http://manual.greatreadingadventure.com/) automatically when they are pulled into the project. If you still wish to build the manual locally, the process is similar to that outlined in the "Building the Docs" section of [the ASP.NET documentation build instructions](https://github.com/aspnet/Docs/blob/master/CONTRIBUTING.md).
