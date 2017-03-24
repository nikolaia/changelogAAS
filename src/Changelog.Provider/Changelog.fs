module Changelog

open Octopus
open Teamcity
open Jira

open FSharp.Data
open FSharp.Data.HttpRequestHeaders
open System.Text.RegularExpressions

let getChangesBetweenEnvironments (args : ChangelogParameters) = 

    let fromEnvVersion = determineEnvironmentVersion args.ProjectName args.fromEnvironment args.octoBaseUrl args.octoApiKey
    let toEnvVersion = determineEnvironmentVersion args.ProjectName args.toEnvironment args.octoBaseUrl args.octoApiKey

    printfn "Fetching changes between version %s (%s) and %s (%s)" toEnvVersion args.toEnvironment fromEnvVersion args.fromEnvironment

    let changes = getChangeDiff args.ProjectName fromEnvVersion toEnvVersion args.tcBaseUrl args.tcUsername args.tcPassword

    let noDots (s : string) = s.Replace(".","")

    let commitMessages = 
        changes
        |> Seq.filter (fun (version, _, _) -> 
            noDots version <= noDots fromEnvVersion
            && noDots version >= noDots toEnvVersion)
        |> Seq.collect (fun (_, _, changes) -> changes)
        |> Seq.map (fun c -> c.Comment)

    let jiraKeys = 
        commitMessages
        |> Seq.choose (fun commit ->
            match commit with
            | jira when jira.Contains "PTWOUTV-" -> Some jira
            | _ -> None )
        |> Seq.map (fun commit ->
            let pattern = "PTWOUTV-(\d+)"
            let number = Regex.Match(commit, pattern).Groups.[1].Value
            sprintf "PTWOUTV-%s" number )

    let jiraIssues = getJiraIssues jiraKeys args.jiraBaseUrl args.jiraUsername args.jiraPassword

    let tryParseMergeCommit (commit : string) =
        match commit with
        | merge when merge.Contains "Merge pull request #" -> Some merge
        | _ -> None

    let tryParseJiraIssueInCommit (commit : string) =
        match commit with
        | jira when jira.Contains "PTWOUTV-" -> Some jira
        | _ -> None


    let getMergeCommitsFromString mergeCommit = 
        let matches = Regex.Match(mergeCommit, "Merge pull request #(\d+) from (?:.+\/)(.+[\n\r]+)\s+((.|[\s])+)$")
        let number = matches.Groups.[1].Value
        let branch = matches.Groups.[2].Value
        let message = matches.Groups.[3].Value
        {
            number = number
            message = message
            link = sprintf "https://github.com/Utdanningsdirektoratet/Prover-hoved/pull/%s" number
        }

    let commitWithDbMigration (commit : string) =
        match commit with
        | _ when commit.Contains("#dbmigration") -> true
        | _ -> false

    let packageHasDbMigration = 
        commitMessages
        |> Seq.exists commitWithDbMigration

    let mergeCommits =
        commitMessages
        |> Seq.choose tryParseMergeCommit
        |> Seq.map getMergeCommitsFromString

    {
        Commits = mergeCommits
        Issues = jiraIssues
        HasDbMigration = packageHasDbMigration
    }