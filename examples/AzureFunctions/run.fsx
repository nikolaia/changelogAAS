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

let Run (req: HttpRequestMessage , log: TraceWriter) : Task<HttpResponseMessage> =
    async { 
        let queryParams = 
            req.GetQueryNameValuePairs()
                .ToDictionary((fun p -> p.Key), (fun p -> p.Value), StringComparer.OrdinalIgnoreCase)

        log.Verbose(sprintf "F# HTTP trigger function processed a request. Name=%A" req.RequestUri)

        let input = 
            queryParams.TryGetValue("projectName"), 
            queryParams.TryGetValue("oldestEnvironment"),
            queryParams.TryGetValue("newestEnvironment") 

        let res =
            match input with 
            | (true, pName),(true, toEnv),(true, fromEnv) -> 

                let parameters = {
                    ProjectName = pName
                    fromEnvironment = fromEnv
                    toEnvironment = toEnv
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
                new HttpResponseMessage(HttpStatusCode.OK, Content = new StringContent(changes.ToString())) // JSON Serialize
            | _ -> 
                new HttpResponseMessage(HttpStatusCode.BadRequest, Content = new StringContent("Please pass newestEnvironment and oldestEnvironment as Query Params"))

        return res
    } |> Async.StartAsTask