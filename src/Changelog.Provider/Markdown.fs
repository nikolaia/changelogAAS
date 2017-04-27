module MarkdownToHtml

open CommonMark
open System.Net
open System.Net.Http
open System.Text

let addNewline s = s + System.Environment.NewLine

let getJiraMarkdownStringFromIssue issue =
    sprintf "* [%s](https://jira.udir.no/browse/%s) %s (_%s_, _%s_, _%s_, _%s_)" 
        issue.Key issue.Key issue.Summary issue.Issuetype issue.FixVersions issue.Labels issue.Status 
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

let markdownAsHtmlPageResponse parameters changes =
    let content = (sprintf """# Changelog for *%s*
This list shows changes between the environments *%s* (%s) and *%s* (%s)
## JIRA Issues

%s

## Mergecommits:

%s

## Database Migrations

%s""" parameters.ProjectName 
parameters.fromEnvironment 
changes.FromVersion 
parameters.toEnvironment 
changes.ToVersion (jiraIssuesMarkdown changes.Issues) (getMergeCommit changes.Commits) (packageHasDbMigration changes.HasDbMigration))

    let body = CommonMark.CommonMarkConverter.Convert(content);
    let site = sprintf """
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
    let bytes = Encoding.Default.GetBytes(site);
    new HttpResponseMessage(HttpStatusCode.OK, Content = new StringContent(Encoding.UTF8.GetString(bytes),Encoding.UTF8,"text/html"))