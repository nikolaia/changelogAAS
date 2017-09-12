module MarkdownToHtml

#if INTERACTIVE

#I "/usr/local/lib/node_modules/azure-functions-core-tools/bin/"
#r "Microsoft.Azure.Webjobs.Host.dll"
#r "System.Net.Http.dll"
#r "System.Net.Http.Formatting.dll"
#I "../../../packages/CommonMark.NET/lib/net45/"
#r "CommonMark.dll"
#load "../../../src/Changelog.Provider/Types.fs"

#else

#r "CommonMark.dll"
#load "Types.fs"

#endif

open CommonMark
open System.Net
open System.Net.Http
open System.Text

let addNewline s = s + System.Environment.NewLine                    

let getJiraMarkdownStringFromIssue issue =
    let color = IssueType.GetColor issue.Issuetype

    sprintf "* [%s](https://%s/browse/%s) %s (<font color='%s'>__%s_</font>,_%s_, _%s_, _%s_)" 
        issue.Link
        issue.Key 
        issue.Key 
        issue.Summary 
        color 
        (issue.Issuetype.ToString()) 
        issue.FixVersions 
        issue.Labels 
        issue.Status
        
let jiraIssuesMarkdown issues =
    issues
    |> Seq.map (getJiraMarkdownStringFromIssue >> addNewline)
    |> Seq.fold (+) ""

let getMergeMarkdownStringFromCommit commit = 
    sprintf "* %s (#%s)" commit.message commit.number
let getMergeCommit commits = 
    commits
    |> Seq.map (getMergeMarkdownStringFromCommit >> addNewline)
    |> Seq.fold (+) "" 

let packageHasDbMigration b = 
    match b with
    | true -> "This package changes the SQL schema and possibly data"
    | false -> "No changes"

let changelogToHtml (changelog : Changelog) =
    let content = (sprintf """# Changelog for *%s*
This list shows changes between the environments *%s* (%s) and *%s* (%s)
## JIRA Issues

%s

## Mergecommits:

%s

## Database Migrations

%s""" changelog.ProjectName
changelog.FromEnvironment.Name
changelog.FromEnvironment.Version
changelog.ToEnvironment.Name
changelog.ToEnvironment.Version (jiraIssuesMarkdown changelog.Issues) (getMergeCommit changelog.Commits) (packageHasDbMigration changelog.HasDbMigration))

    let body = CommonMark.CommonMarkConverter.Convert(content);
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
    