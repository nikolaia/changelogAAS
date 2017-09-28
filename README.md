# Changelog-as-a-Service
[![Build status](https://ci.appveyor.com/api/projects/status/b3s0h5tulgtlnssu?svg=true)](https://ci.appveyor.com/project/nikolaia/changelogaas)

Integrate with the APIs of your deployment pipeline to generate changelogs to prevent adding changelog generation into the pipeline itself.

Currently works with a pipeline consisting of Octopus Deploy (OD), TeamCity (TC) and JIRA, but it should be easy to swap any of them out. The goal is to make an adaptable solution where you choose which systems you want to use.

## Hosting / Usage examples

For different examples of hosting the provider (Azure Functions, Suave etc.), check out the `examples` folder.

Simple example calling the library from C#:
```csharp
var mapping = new List<Types.ProjectMapping>() {
  new Types.ProjectMapping(
    teamcityName: "Example_ReleaseExample",
    octoDeployName: "Example.Project",
    githubUrl: "https://github.com/Organisation/ExampleProject",
    jiraKey: "EXA1") };

var changelogParameteres = new Types.ChangelogParameters(
  jiraUrl: "https://jira.example.com",
  teamcityUrl: "https://teamcity.example.com",
  octoUrl: "https://octopusdeploy.example.com",
  octoApiKey: octoApiKey,
  tcUsername: tcUsername,
  tcPassword: tcPassword,
  jiraUsername: jiraUsername,
  jiraPassword: jiraPassword,
  projectMappings: mapping,
  projectName: projectName,
  fromEnvironmentName: fromEnvironment,
  toEnvironmentName: toEnvironment);

var changelog = Changelog.getChangesBetweenEnvironments(changelogParameteres);
return HumanReadable.changelogToHtml(changelog);
```

## TODO

* More documentation and examples
* Publish provider on NuGet
* Make integrations into adapters/providers to support more sources
* Add ARM-template for Azure Functions infrastructure in example