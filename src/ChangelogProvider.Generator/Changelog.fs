module Changelog

open System
open Octopus
open Teamcity
open Jira
open System.Text.RegularExpressions


let getChangesBetweenVersions (args : ChangelogParameters) = 
    
    printfn "Fetching changes between version %s and %s" args.fromVersion args.toVersion

    let currentProject =
        args.projectMappings
        |> Seq.tryFind (fun p -> p.octoDeployName = args.projectName)
        |> function
            | Some p -> p
            | None -> failwith (sprintf "Unable find project mappings for project %s. Please make sure you config.yaml is correct." args.projectName)

    // Get all builds from the Teamcity API that are the versions in questions or a version between them
    let builds = getChangeDiff currentProject.teamcityName args.toVersion args.fromVersion args.teamcityUrl args.tcUsername args.tcPassword

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
            let githubUrl = currentProject.githubUrl
            {
                number = number
                message = message
                link =  Uri (githubUrl, (sprintf "pull/%s" number))
            })

    {
        ProjectName = args.projectName
        FromVersion = args.fromVersion 
        ToVersion = args.toVersion
        Commits = mergeCommits
        Issues = jiraIssues
        HasDbMigration = packageHasDbMigration
    }

let getChangesBetweenEnvironments (args : EnvironmentChangelogParameters) = 
    // Fetch the two version we want to diff based on our two environment names
    printfn "Fetching changes between environments %s and %s" args.fromEnvironmentName args.toEnvironmentName

    let highestVersion = determineEnvironmentVersion args.projectName args.fromEnvironmentName args.octoUrl args.octoApiKey
    let lowestVersion = determineEnvironmentVersion args.projectName args.toEnvironmentName args.octoUrl args.octoApiKey
    
    let parameters = {
        projectName = args.projectName
        fromVersion = lowestVersion
        toVersion = highestVersion
        tcUsername = args.tcUsername
        tcPassword = args.tcPassword 
        jiraUsername = args.jiraUsername
        jiraPassword = args.jiraPassword
        teamcityUrl = args.teamcityUrl
        jiraUrl = args.jiraUrl
        projectMappings = args.projectMappings
    }
    
    getChangesBetweenVersions parameters