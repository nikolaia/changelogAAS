# Changelog-as-a-Service

Integrate with the APIs of your deployment pipeline to generate changelogs to prevent adding changelog generation into the pipeline itself.

Currently works with a pipeline consisting of Octopus Deploy (OD), TeamCity (TC) and JIRA, but it should be easy to swap any of them out. The goal is to make a pluggable solution where you choose which systems you want to use.

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

## Hosting

There are three simple Azure Functions for hosting it on Azure Functions (`src/AzureFunctions`). It uses queues to make a response fast for whatever system is triggering the generation and uses SendGrid in the other end to send an e-mail with the changelog to a given address. Generated changelog are saved as a JSON-document in Cosmos DB.

There is also a Suave example for self-hosting at `src/Changelog.Suave/` which can produce both JSON and human readable text.

## TODO

* More documentation and examples
* Publish provider on NuGet
* Make adapters/providers pluggable to support more sources
* Add ARM-template for Azure Functions infrastructure
