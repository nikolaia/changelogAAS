//----------------------------------------------------------------------------------------
// This prelude allows scripts to be edited in Visual Studio or another F# editing environment 

#if !COMPILED

#I @"../../packages/Microsoft.Azure.WebJobs/lib/net45/"
#r "Microsoft.Azure.WebJobs.Host.dll"

#I @"../../packages/FSharp.Data/lib/net40/"
#r "FSharp.Data.dll"

#I @"../../build/"
#r "ChangelogAAS.dll"

#I @"../../packages/System.Net.Http/lib/net46"
#r "System.Net.Http.dll"

#endif

//----------------------------------------------------------------------------------------
// This is the body of the function 

open Changelog

open System
open System.Linq
open System.Net
open System.Net.Http
open System.Threading.Tasks
open Microsoft.Azure.WebJobs.Host

(*
    let octoApiKey = System.Environment.GetEnvironmentVariable("OCTO_API_KEY")
    let octoBaseUrl = "https://oslaz-pas2-depl.udir.no"

    let tcBaseUrl = "https://oslaz-pas2-int.udir.no"
    let tcUsername = System.Environment.GetEnvironmentVariable("TEAMCITY_USERNAME")
    let tcPassword = System.Environment.GetEnvironmentVariable("TEAMCITY_PASSWORD")

    let jiraBaseUrl = "https://pashjelp.udir.no"
    let jiraUsername = System.Environment.GetEnvironmentVariable("JIRA_USERNAME")
    let jiraPassword = System.Environment.GetEnvironmentVariable("JIRA_PASSWORD")

*)

let Run (req: HttpRequestMessage , log: TraceWriter) : Task<HttpResponseMessage> =
    async { 
        let queryParams = req.GetQueryNameValuePairs().ToDictionary((fun p -> p.Key), (fun p -> p.Value), StringComparer.OrdinalIgnoreCase)

        log.Verbose(sprintf "F# HTTP trigger function processed a request. Name=%A" req.RequestUri)

        let projectName = queryParams.TryGetValue("projectName")
        let newestEnvironment = queryParams.TryGetValue("newestEnvironment")
        let oldestEnvironment = queryParams.TryGetValue("oldestEnvironment")

        let res =
            match projectName, newestEnvironment, oldestEnvironment with 
            | (true, pName),(true, newEnv),(true, oldEnv) -> 
                let changes = Changelog.getChangesBetweenEnvironments pName newEnv oldEnv
                new HttpResponseMessage(HttpStatusCode.OK, Content = new StringContent(changes))
            | _ -> 
                new HttpResponseMessage(HttpStatusCode.BadRequest, Content = new StringContent("Please pass newestEnvironment and oldestEnvironment as Query Params"))

        return res
    } |> Async.StartAsTask