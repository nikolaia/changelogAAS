namespace AspNetCore.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Changelog

type HomeController () =
    inherit Controller()

    member this.Index () =
        this.View();

    [<Route("/changes")>]
    member this.Changes () =
        
        let parameters = {
            ProjectName = ""
            fromEnvironment = ""
            toEnvironment = ""
            octoApiKey = System.Environment.GetEnvironmentVariable("OCTO_API_KEY")
            octoBaseUrl = "https://oslaz-pas2-depl.udir.no"
            tcBaseUrl = "https://oslaz-pas2-int.udir.no"
            tcUsername = System.Environment.GetEnvironmentVariable("TEAMCITY_USERNAME")
            tcPassword = System.Environment.GetEnvironmentVariable("TEAMCITY_PASSWORD")
            jiraBaseUrl = "https://pashjelp.udir.no"
            jiraUsername = System.Environment.GetEnvironmentVariable("JIRA_USERNAME")
            jiraPassword = System.Environment.GetEnvironmentVariable("JIRA_PASSWORD")
        }

        let changes = Changelog.getChangesBetweenEnvironments parameters

        JsonResult(changes)

    member this.Error () =
        this.View();
