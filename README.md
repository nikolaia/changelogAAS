# Changelog-as-a-Service
[![Build status](https://ci.appveyor.com/api/projects/status/ngga2c61odbj71io/branch/master?svg=true)](https://ci.appveyor.com/project/nikolaia/changelogaas/branch/master)

Integrate with the APIs of your deployment pipeline to generate changelogs to prevent adding changelog generation into the pipeline itself.

Currently works with a pipeline consisting of Octopus Deploy (OD), TeamCity (TC) and JIRA, but it should be easy to swap any of them out. The goal is to make an adaptable solution where you choose which systems you want to use.

## Configuration

The `Changelog.Provider` library uses `config.yaml` to map projects between OD, TC and JIRA. JIRA Work Items mentioned in commit comments will be parsed and fetched from the JIRA API. The `config.yaml` is read in at execution time, and needs to be present.

An example of `config.yaml`:

```yaml
---
jiraUrl: "https://jira.example.com"
octoUrl: "https://octopusdeploy.example.com"
projects:
  -
    githubUrl: "https://github.com/github/example1"
    jiraKey: EXA1
    octoDeployName: EXAMPLE1
    teamcityName: Example1_BuildAndRelease
  -
    githubUrl: "https://github.com/github/example2"
    jiraKey: EXA2
    octoDeployName: EXAMPLE2
    teamcityName: Example2_BuildAndRelease
teamcityUrl: "https://teamcity.example.com"
```

## Hosting / Usage examples

For different examples of hosting the provider (Azure Functions, Suave etc.), check out the `examples` folder.

## TODO

* More documentation and examples
* Publish provider on NuGet
* Make integrations into adapters/providers to support more sources
* Add ARM-template for Azure Functions infrastructure in example