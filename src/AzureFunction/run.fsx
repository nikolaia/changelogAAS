//----------------------------------------------------------------------------------------
// This prelude allows scripts to be edited in Visual Studio or another F# editing environment 

#if !COMPILED
#r "Microsoft.Azure.WebJobs.Host"
#r "System.Net.Http"
#endif

//----------------------------------------------------------------------------------------
// This is the body of the function 

#load "changelog.fsx"
#load "mdhtml.fsx"

open Changelog
open MarkdownToHtml

open System
open System.Linq
open System.Net
open System.Net.Http
open System.Threading.Tasks
open Microsoft.Azure.WebJobs.Host

let Run (req: HttpRequestMessage , log: TraceWriter) : Task<HttpResponseMessage> =
    async { 
        let queryParams = req.GetQueryNameValuePairs().ToDictionary((fun p -> p.Key), (fun p -> p.Value), StringComparer.OrdinalIgnoreCase)

        log.Verbose(sprintf "F# HTTP trigger function processed a request. Name=%A" req.RequestUri)

        let projectName = queryParams.TryGetValue("projectName")
        let newestEnvironment = queryParams.TryGetValue("newestEnvironment")
        let oldestEnvironment = queryParams.TryGetValue("oldestEnvironment")
        let toHtml = queryParams.TryGetValue("toHtml")

        let res =
            match projectName, newestEnvironment, oldestEnvironment with 
            | (true, pName),(true, newEnv),(true, oldEnv) -> 
                let changes = Changelog.getChangesBetweenEnvironments pName newEnv oldEnv
                match toHtml with
                | (true, "true") ->
                    MarkdownToHtml.markdownAsHtmlPageResponse changes
                | _ -> new HttpResponseMessage(HttpStatusCode.OK, Content = new StringContent(changes))
            | _ -> 
                new HttpResponseMessage(HttpStatusCode.BadRequest, Content = new StringContent("Please pass newestEnvironment and oldestEnvironment as Query Params"))

        return res
    } |> Async.StartAsTask