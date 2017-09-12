#if INTERACTIVE

#I "/usr/local/lib/node_modules/azure-functions-core-tools/bin/"
#r "Microsoft.Azure.Webjobs.Host.dll"
#r "System.Net.Http.dll"
#r "System.Net.Http.Formatting.dll"
#I "../../../packages/Microsoft.AspNet.WebApi.WebHost/lib/net45/"
#r "System.Web.Http.WebHost.dll"

#I "../../../build/Provider"

#endif

#r "Changelog.Provider.dll"
#r "Newtonsoft.Json"

open System
open System.Text
open System.Linq
open System.Net
open System.Net.Http
open System.Threading.Tasks
open Microsoft.Azure.WebJobs.Host
open Newtonsoft.Json

let Run (req: HttpRequestMessage , log: TraceWriter) =

        log.Verbose(sprintf "F# HTTP trigger function processed a request. Name=%A" req.RequestUri)
 
        let queryParams = 
            req.GetQueryNameValuePairs()
                .ToDictionary((fun p -> p.Key), (fun p -> p.Value), StringComparer.OrdinalIgnoreCase)

        let tryGetQueryParam name =
            match queryParams.TryGetValue(name) with
            | (true, value) -> value
            | (false, _) -> failwithf "Unable to parse query value %A" name

        let input = {
            ProjectName = tryGetQueryParam "projectName"
            FromEnvironmentName = tryGetQueryParam "fromEnv"
            ToEnvironmentName = tryGetQueryParam "toEnv"
        }
        JsonConvert.SerializeObject input