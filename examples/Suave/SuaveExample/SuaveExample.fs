module MyWebApi.Program

open Suave
open Suave.Successful
open Suave.Writers
open Suave.Operators
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Changelog

[<EntryPoint>]
let main argv =

    let parameters = {
        ProjectName = "UDIR.Prover"
        fromEnvironment = "PAS2-QA"
        toEnvironment = "PAS2-PROD"
        octoApiKey = System.Environment.GetEnvironmentVariable("OCTO_API_KEY")
        octoBaseUrl = "https://oslaz-pas2-depl.udir.no"
        tcBaseUrl = "https://oslaz-pas2-int.udir.no"
        tcUsername = System.Environment.GetEnvironmentVariable("TEAMCITY_USERNAME")
        tcPassword = System.Environment.GetEnvironmentVariable("TEAMCITY_PASSWORD")
        jiraBaseUrl = "https://pashjelp.udir.no"
        jiraUsername = System.Environment.GetEnvironmentVariable("JIRA_USERNAME")
        jiraPassword = System.Environment.GetEnvironmentVariable("JIRA_PASSWORD")
    }

    let getChanges() : WebPart =
        let changes = Changelog.getChangesBetweenEnvironments parameters
        let jsonSerializerSettings = new JsonSerializerSettings()
        jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
        
        JsonConvert.SerializeObject(changes, jsonSerializerSettings) 
        |> OK
        >=> setMimeType "application/json; charset=utf-8"

    startWebServer defaultConfig (getChanges())
    0
