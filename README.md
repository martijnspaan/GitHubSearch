# GitHubSearch - A commandline search utilizing the GitHub Api

Born from the limitation that the online [GitHub search](https://help.github.com/articles/searching-code/) has, where files larger than 384 KB are not indexed. This commandline search will download (and cache) the files before searching.

## How to install
The easiest deployment mechanism is to install [Chocolatey](https://chocolatey.org/) and run the following command-line to install the [Chocolate package](https://chocolatey.org/packages/githubsearch):

	choco install githubsearch

Then run `GitHubSearch` or short `ghs` and observe the command-line arguments.

Alternatively you can download the latest stable release: [GitHubSearch 1.2.0](https://github.com/martijnspaan/GitHubSearch/releases/download/1.2.0/GitHubSearch_1.2.0.zip)

## License 

Licensed under the [MIT License](https://github.com/martijnspaan/GitHubSearch/blob/master/LICENSE)

Thanks to all the awesome libraries making this searcher possible:
* [Octokit](https://github.com/octokit/octokit.net)
* [Colorful.Console](https://github.com/tomakita/Colorful.Console)
* [NewtonSoft](https://github.com/JamesNK/Newtonsoft.Json)
* [TinyIoC](https://github.com/grumpydev/TinyIoC)

[![Build status](https://ci.appveyor.com/api/projects/status/psvicycmoj71go6n/branch/master?svg=true)](https://ci.appveyor.com/project/martijnspaan/githubsearch/branch/master)

Test Changes