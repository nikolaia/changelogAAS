module HumanReadable

open Changelog
open CommonMark
open System.Net
open System.Net.Http
open System.Text

let private addNewline s = s + System.Environment.NewLine                    

let private getJiraMarkdownStringFromIssue issue =
    let color = IssueType.GetColor issue.Issuetype

    sprintf "* [%s](%O) %s (<font color='%s'>_%s_</font>,_%s_, _%s_, _%s_)" 
        issue.Key 
        issue.Link
        issue.Summary 
        color 
        (IssueType.ToString issue.Issuetype) 
        issue.FixVersions 
        issue.Labels 
        issue.Status
        
let private jiraIssuesMarkdown issues =
    issues
    |> Seq.map (getJiraMarkdownStringFromIssue >> addNewline)
    |> Seq.fold (+) ""

let private getMergeMarkdownStringFromCommit commit = 
    sprintf "* [%s](%O) %s" commit.number commit.link commit.message

let private getMergeCommit commits = 
    commits
    |> Seq.map (getMergeMarkdownStringFromCommit >> addNewline)
    |> Seq.fold (+) "" 

let private packageHasDbMigration b = 
    match b with
    | true -> "This package changes the SQL schema and possibly data"
    | false -> "No changes"

let changelogToMarkdown (changelog : Changelog) =
    (sprintf """# Changelog for *%s*
This list shows changes between version *%s* and *%s*
## JIRA Issues

%s

## Mergecommits:

%s

## Database Migrations

%s""" changelog.ProjectName
changelog.FromVersion
changelog.ToVersion (jiraIssuesMarkdown changelog.Issues) (getMergeCommit changelog.Commits) (packageHasDbMigration changelog.HasDbMigration))

let changelogToHtml (changelog : Changelog) =
    let body = CommonMark.CommonMarkConverter.Convert(changelogToMarkdown changelog);
    sprintf """
        <!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
        <html lang="en">
        <head>
            <meta http-equiv="content-type" content="text/html; charset=utf-8">
            <title>Changelog</title>
            <!-- Bootstrap core CSS -->
            <link href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" rel="stylesheet">
        </head>
        <body>
            <div class="container">
                %s
            </div>
        </body>
        </html>""" body