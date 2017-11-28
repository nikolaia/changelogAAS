module Jira

open System
open FSharp.Data
open JiraSamples
open FSharp.Data.HttpRequestHeaders

type JiraIssueSearchJson = JsonProvider<JiraIssueSearchSample>

let getJiraIssues keys baseUrl username password = 
    if keys |> Seq.isEmpty then
         Seq.empty
    else
        let buildQuery queryKeys = 
            (sprintf """ 
            {  
                "jql":"key in (%s)",
                "startAt":0,
                "fields":[  
                    "id",
                    "key",
                    "fixVersions",
                    "customfield_10205",
                    "labels",
                    "issuetype",
                    "summary",
                    "status"
                ]
            }
            """
            (queryKeys |> String.concat ","))

        let url = Uri (baseUrl,"/rest/api/2/search/")
        
        let header = [ BasicAuth username password; Accept HttpContentTypes.Json; ContentType "application/json;charset=utf-8" ]

        let doSearch searchKeys = 
            try
                Some (Http.RequestString(url.ToString(), 
                                         headers = header,
                                         httpMethod = "POST", 
                                         body = TextRequest (buildQuery searchKeys)))
            with
                | :? System.Net.WebException as ex -> printfn "Error: %s" ex.Message ; None

        let parsedIssues = 
            match doSearch keys with
            | Some res -> JiraIssueSearchJson.Parse res |> fun i -> i.Issues
            | None ->
                printfn "Error: Unable to fetch all Jira keys in one request. Attempting to fetch them one by one."
                keys
                |> Seq.choose (fun i -> match doSearch [i] with
                                        | Some issue -> Some (issue |> JiraIssueSearchJson.Parse |> fun p -> p.Issues)
                                        | None -> None )
                |> Seq.concat
                |> Seq.toArray

        parsedIssues
        |> Seq.map (fun issue -> 
            let fixVersions =
                match issue.Fields.JsonValue.TryGetProperty("fixVersions") with
                | Some p when p <> JsonValue.Null -> issue.Fields.FixVersions |> Seq.map (fun el -> el.Name) |> String.concat ", "
                | _ -> ""

            let applicationUser =
                match issue.Fields.JsonValue.TryGetProperty("customfield_10205") with
                | Some p when p <> JsonValue.Null -> issue.Fields.Customfield10205 |> Seq.map (fun el -> el.Value) |> String.concat ", "
                | _ -> ""
                                
            { 
                Key = issue.Key
                Summary = issue.Fields.Summary
                Issuetype = IssueType.FromString issue.Fields.Issuetype.Name
                FixVersions = fixVersions
                ApplicationUser = applicationUser
                Labels = issue.Fields.Labels |> String.concat ","
                Status = issue.Fields.Status.Name
                Link = Uri(baseUrl, (sprintf "browse/%s" issue.Key))
            })