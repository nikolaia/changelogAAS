module Changelog

open Octopus
open Teamcity
open Jira

open FSharp.Data
open FSharp.Data.HttpRequestHeaders
open System.Text.RegularExpressions


let getChangesBetweenEnvironments (args : ChangelogParameters) = 
    
    // Fetch the two version we want to diff based on our two environment names
    let fromEnvVersion = determineEnvironmentVersion args.projectName args.fromEnvironmentName args.octoUrl args.octoApiKey
    let toEnvVersion = determineEnvironmentVersion args.projectName args.toEnvironmentName args.octoUrl args.octoApiKey

    printfn "Fetching changes between version %s (%s) and %s (%s)" toEnvVersion args.toEnvironmentName fromEnvVersion args.fromEnvironmentName

    // Get the current projects mappings from the Yaml config file 
    // (To know what the project is named in the different systems)
    let currentProject =
        args.projectMappings
        |> Seq.tryFind (fun p -> p.octoDeployName = args.projectName)
        |> function
            | Some p -> p
            | None -> failwith (sprintf "Unable find project mappings for project %s. Please make sure you config.yaml is correct." args.projectName)

    // Get all builds from the Teamcity API that are the versions in questions or a version between them
    let builds = getChangeDiff currentProject.teamcityName fromEnvVersion toEnvVersion args.teamcityUrl args.tcUsername args.tcPassword

    // Get the commitmessages for all the builds
    let commitMessages = 
        builds
        |> Seq.collect (fun b -> b.Comments)

    // Based on the JIRA codes provided in the Yaml config file, find all
    // JIRA tasks mentioned in the commitmessages (This will include tasks
    // from all projects, not just the provided projectName)
    let jiraKeys = 
        commitMessages
        |> Seq.collect (fun commit ->
            args.projectMappings
            |> Seq.map (fun p -> p.jiraKey)
            |> String.concat "|"
            |> sprintf "(%s)-(\d+)"
            |> fun pattern -> seq { for x in Regex.Matches(commit,pattern) do yield x.Value } )
        |> Seq.distinct
    
    // Fetch more information about the issues parsed from the commit-
    // messages from the JIRA API
    let jiraIssues = getJiraIssues jiraKeys args.jiraUrl args.jiraUsername args.jiraPassword

    let packageHasDbMigration = 
        commitMessages
        |> Seq.exists (fun commit -> commit.Contains("#dbmigration"))

    // Use Regex to find the Pull Request numbers so we can create a
    // link to the github project.
    let mergeCommits =
        commitMessages
        |> Seq.filter (fun commit -> commit.Contains "Merge pull request #")
        |> Seq.map (fun mergeCommit ->
            let matches = Regex.Match(mergeCommit, "Merge pull request #(\d+) from (?:.+\/)(.+[\n\r]+)\s+((.|[\s])+)$")
            let number = matches.Groups.[1].Value
            let branch = matches.Groups.[2].Value
            let message = matches.Groups.[3].Value
            let githubUrl = currentProject.githubUrl.ToString()
            {
                number = number
                message = message
                link = sprintf "%spull/%s" githubUrl number
            })

    {
        ProjectName = args.projectName
        FromEnvironment = { Name = args.fromEnvironmentName; Version = fromEnvVersion }
        ToEnvironment = { Name = args.toEnvironmentName; Version = toEnvVersion }
        Commits = mergeCommits
        Issues = jiraIssues
        HasDbMigration = packageHasDbMigration
    }