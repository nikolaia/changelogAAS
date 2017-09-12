#if INTERACTIVE

#I "/usr/local/lib/node_modules/azure-functions-core-tools/bin/"
#r "Microsoft.Azure.Webjobs.Host.dll"
#r "System.Net.Http.dll"
#r "System.Net.Http.Formatting.dll"
#I "../../../packages/Microsoft.AspNet.WebApi.WebHost/lib/net45/"
#r "System.Web.Http.WebHost.dll"

#I "../../../build/Provider"

#endif

#r "Newtonsoft.Json"
#r "Changelog.Provider.dll"

open Changelog
open System
open System.Text
open System.Linq
open System.Net
open System.Net.Http
open System.Threading.Tasks
open Microsoft.Azure.WebJobs.Host
open Newtonsoft.Json

let Run (changelogMsg : string) (changelogJson : byref<string>, (changelogId : byref<string>)) =
   
    // check for config.yaml

    let input = JsonConvert.DeserializeObject<ChangelogInput>(changelogMsg)

    let parameters = {
        input = input
        octoApiKey = System.Environment.GetEnvironmentVariable("OCTO_API_KEY")
        tcUsername = System.Environment.GetEnvironmentVariable("TEAMCITY_USERNAME")
        tcPassword = System.Environment.GetEnvironmentVariable("TEAMCITY_PASSWORD")
        jiraUsername = System.Environment.GetEnvironmentVariable("JIRA_USERNAME")
        jiraPassword = System.Environment.GetEnvironmentVariable("JIRA_PASSWORD")
    }

    let changelog = Changelog.getChangesBetweenEnvironments parameters
    changelogId <- 
        sprintf "%s-%s-%s" 
            changelog.ProjectName 
            changelog.FromEnvironment.Version 
            changelog.ToEnvironment.Version

    changelogJson <- JsonConvert.SerializeObject(changelog)
    
    id