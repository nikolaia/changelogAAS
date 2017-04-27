module Changelog.Suave

open Suave
open Suave.Successful
open Suave.Writers
open Suave.Operators
open Suave.Filters
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open System.IO
open Changelog
open MarkdownToHtml

let parameters = {
    ProjectName = "UDIR.PAS2"
    fromEnvironment = "PAS2-QA"
    toEnvironment = "PAS2-PROD"
    octoApiKey = System.Environment.GetEnvironmentVariable("OCTO_API_KEY")
    octoBaseUrl = "https://oslaz-pas2-od.udir.no"
    tcBaseUrl = "https://oslaz-pas2-int.udir.no"
    tcUsername = System.Environment.GetEnvironmentVariable("TEAMCITY_USERNAME")
    tcPassword = System.Environment.GetEnvironmentVariable("TEAMCITY_PASSWORD")
    jiraBaseUrl = "https://jira.udir.no"
    jiraUsername = System.Environment.GetEnvironmentVariable("JIRA_USERNAME")
    jiraPassword = System.Environment.GetEnvironmentVariable("JIRA_PASSWORD")
}

let getChanges() : WebPart =
  fun (ctx : HttpContext) ->
    async {
        let changes = Changelog.getChangesBetweenEnvironments parameters
        let jsonSerializerSettings = JsonSerializerSettings()
        jsonSerializerSettings.ContractResolver <- CamelCasePropertyNamesContractResolver()
        let json = JsonConvert.SerializeObject(changes, jsonSerializerSettings) 
        return! OK json ctx
    }

let getChangesAsMarkdown() : WebPart =
  fun (ctx : HttpContext) ->
    async {
        let changes = Changelog.getChangesBetweenEnvironments parameters
        let response = MarkdownToHtml.markdownAsHtmlPageResponse parameters changes
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        return! OK content ctx
    }

[<EntryPoint>]
let main argv =
    let app =
          choose [ 
            GET >=> path "/api" >=> getChanges() >=> setMimeType "application/json; charset=utf-8"
            GET >=> path "/" >=> getChangesAsMarkdown()
            GET >=> Files.browseHome
            RequestErrors.NOT_FOUND "Page not found." 
          ]

    let config =
        { defaultConfig with homeFolder = Some (Path.GetFullPath "./wwwroot") }

    startWebServer config app
    0