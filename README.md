# GitHubSearch - A commandline search utilizing the GitHub Api

Born from the limitation that the online [GitHub search](https://help.github.com/articles/searching-code/) has, where files larger than 384 KB are not indexed. This commandline search will download (and cache) the files before searching.

[![Build status](https://ci.appveyor.com/api/projects/status/g9lj2yk6efj5fibe?svg=true)](https://ci.appveyor.com/project/martijnspaan/githubsearch)

## How to install
The easiest deployment mechanism is to install [Chocolatey](https://chocolatey.org/) and run the following command-line to install the [Chocolate package](https://chocolatey.org/packages/githubsearch):

	choco install githubsearch

Then run `GitHubSearch` or short `ghs` and observe the command-line arguments.

## License 

Licensed under the [MIT License](https://github.com/martijnspaan/GitHubSearch/blob/master/LICENSE)

Thanks to all the awesome libraries making this searcher possible:
* [Octokit](https://github.com/octokit/octokit.net)
* [Colorful.Console](https://github.com/tomakita/Colorful.Console)
* [TinyIoC](https://github.com/grumpydev/TinyIoC)
