module Jira

open FSharp.Data
open JiraSamples
open FSharp.Data.HttpRequestHeaders

type JiraIssueSearchJson = JsonProvider<JiraIssueSearchSample>

let getJiraIssues keys baseUrl username password = 
    if keys |> Seq.isEmpty then
         Seq.empty
    else
        let query = 
            sprintf """ {"jql":"key in (%s)","startAt":0,"fields":["id","key","customfield_12121","customfield_12122","issuetype","summary","status"]} """ 
                (keys |> String.concat ",")

        let url = sprintf "/rest/api/2/search/"
        
        let response = Http.RequestString(url, 
                                        headers = [ BasicAuth username password; Accept HttpContentTypes.Json; ContentType "application/json;charset=utf-8" ], 
                                        httpMethod = "POST", 
                                        body = TextRequest query)

        let parsedSearch = JiraIssueSearchJson.Parse response
        
        parsedSearch.Issues
        |> Seq.map (fun issue -> let hendelse = 
                                    match issue.Fields.JsonValue.TryGetProperty("customfield_12122") with
                                            | Some p when p <> JsonValue.Null -> issue.Fields.Customfield12122.Value
                                            | _ -> ""
                                 let omraade =
                                    match issue.Fields.JsonValue.TryGetProperty("customfield_12121") with
                                            | Some p when p <> JsonValue.Null -> issue.Fields.Customfield12121.Value
                                            | _ -> ""

                                 { Key = issue.Key
                                   Summary = issue.Fields.Summary
                                   Issuetype = issue.Fields.Issuetype.Name
                                   Omraade = omraade
                                   Hendelse = hendelse
                                   Status = issue.Fields.Status.Name
                                   Link = sprintf "%s/browse/%s" baseUrl issue.Key } )