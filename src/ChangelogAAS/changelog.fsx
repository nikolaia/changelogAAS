module Changelog

#r "FSharp.Data"
#load "octopus.fsx"
#load "teamcity.fsx"
#load "jira.fsx"
#load "markdown.fsx"

open Octopus
open Teamcity
open Jira

open FSharp.Data
open FSharp.Data.HttpRequestHeaders
open System.Text.RegularExpressions

let getChangesBetweenEnvironments projectName newestEnvironment oldestEnvironment = 

    let oldVersion = determineEnvironmentVersion projectName oldestEnvironment
    let newVersion = determineEnvironmentVersion projectName newestEnvironment

    let changes = getChangeDiff projectName oldVersion newVersion

    (* 
        Parsing and markdown stuff
    *)

    let tryParseMergeCommit (commit : string) =
        match commit with
        | merge when merge.Contains "Merge pull request #" -> Some merge
        | _ -> None

    let tryParseJiraIssueInCommit (commit : string) =
        match commit with
        | jira when jira.Contains "PTWOUTV-" -> Some jira
        | _ -> None

    let trd (_, _, c) = c

    let getMarkdownStringFromMerge mergeCommit = 
        let matches = Regex.Match(mergeCommit, "Merge pull request #(\d+) from (?:.+\/)(.+[\n\r]+)\s+((.|[\s])+)$")
        let number = matches.Groups.[1].Value
        let branch = matches.Groups.[2].Value
        let message = matches.Groups.[3].Value
        sprintf "* [#%s](https://github.com/Utdanningsdirektoratet/Prover-hoved/pull/%s) from branch [%s](https://github.com/Utdanningsdirektoratet/Prover-hoved/tree/%s)" number number message branch

    let getJiraKeyFromCommit commit =
        let pattern = "PTWOUTV-(\d+)"
        let number = Regex.Match(commit, pattern).Groups.[1].Value
        sprintf "PTWOUTV-%s" number

    let getJiraMarkdownStringFromIssue issue =
        sprintf "* [%s](https://pashjelp.udir.no/browse/%s) %s (_%s_, _%s_, _%s_, _%s_)" issue.Key issue.Key issue.Summary issue.Issuetype issue.Omraade issue.Hendelse issue.Status 

    let commitWithDbMigration (commit : string) =
        match commit with
        | _ when commit.Contains("#dbmigration") -> true
        | _ -> false

    (*

    Write to file

    *)
    let commitMessages = 
        changes
        |> Seq.collect trd
        |> Seq.map (fun c -> c.Comment)

    let packageHasDbMigration = 
        commitMessages
        |> Seq.exists commitWithDbMigration
        |> fun b -> match b with
                    | true -> "This package changes the SQL schema and possibly data"
                    | false -> "No changes"

    let addNewline s = s + System.Environment.NewLine

    let mergeCommits =
        commitMessages
        |> Seq.choose tryParseMergeCommit
        |> Seq.map (getMarkdownStringFromMerge >> addNewline)
        |> Seq.fold (+) ""

    let jiraKeys = 
        commitMessages
        |> Seq.choose tryParseJiraIssueInCommit
        |> Seq.map getJiraKeyFromCommit

    let jiraIssues = getJiraIssues jiraKeys

    let jiraIssuesMarkdown =
        jiraIssues
        |> Seq.map (getJiraMarkdownStringFromIssue >> addNewline)
        |> Seq.fold (+) ""

    sprintf """# Changelog for *%s*

This list shows changes between the environments *%s* (Version %s) and *%s* (Version %s)

## JIRA Issues

%s

## Mergecommits:

%s

## Database Migrations

%s""" projectName newestEnvironment newVersion oldestEnvironment oldVersion jiraIssuesMarkdown mergeCommits packageHasDbMigration