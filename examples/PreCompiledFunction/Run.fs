namespace Changelog

module PreCompiledFunction =
    
    open Changelog
    open System
    open System.Text
    open System.Linq
    open System.Net
    open System.Net.Http
    open System.Threading.Tasks
    open Microsoft.Azure.WebJobs.Host
    open Newtonsoft.Json
    
    let Run (req : HttpRequestMessage, style : string, log : TraceWriter) =
       async {
            // check for config.yaml
            
            let! body = req.Content.ReadAsStringAsync() |> Async.AwaitTask
            
            let input = JsonConvert.DeserializeObject<ChangelogInput>(body)
    
            let parameters = {
                input = input
                octoApiKey = System.Environment.GetEnvironmentVariable("OCTO_API_KEY")
                tcUsername = System.Environment.GetEnvironmentVariable("TEAMCITY_USERNAME")
                tcPassword = System.Environment.GetEnvironmentVariable("TEAMCITY_PASSWORD")
                jiraUsername = System.Environment.GetEnvironmentVariable("JIRA_USERNAME")
                jiraPassword = System.Environment.GetEnvironmentVariable("JIRA_PASSWORD")
            }
            
            let changelog = Changelog.getChangesBetweenEnvironments parameters
            
            match style with
            | "markdown" -> return HumanReadable.changelogToMarkdown changelog
            | "html" ->  return HumanReadable.changelogToHtml changelog
            | _ -> return JsonConvert.SerializeObject(changelog)
            
       } |> Async.StartAsTask