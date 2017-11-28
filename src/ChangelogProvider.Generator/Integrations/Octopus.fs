module Octopus

open System
open OctopusSamples
open FSharp.Data
open FSharp.Data.HttpRequestHeaders


type OctoProjectList = JsonProvider<OctopusProjectListSample>
type OctoProject = JsonProvider<OctopusProjectSample>

let determineEnvironmentVersion projectName environmentName (baseUrl:Uri) apiKey  =

    let octoHeader = [ "X-Octopus-ApiKey", apiKey; Accept HttpContentTypes.Json ]

    let httpRequestOctopus (subUrl:string) =
        let url = Uri (baseUrl, subUrl)
        printfn "Octopus Deploy: Calling %O to get information about %s in environment %s" url projectName environmentName
        Http.RequestString(url.ToString(), headers = octoHeader)

    let projectId =
        httpRequestOctopus "api/projects"
        |> OctoProjectList.Parse
        |> fun projectList -> projectList.Items
        |> Seq.tryFind (fun p -> p.Name = projectName)
        |> function
            | Some p -> p.Id
            | None -> failwith (sprintf "Could not find the project %s" projectName)

    let octoProject =
        sprintf "api/progression/%s" projectId
        |> httpRequestOctopus
        |> OctoProject.Parse

    let environmentId =
        octoProject
        |> fun p -> p.Environments
        |> Seq.tryFind (fun e -> e.Name = environmentName)
        |> function
            | Some e -> e.Id
            | None -> failwith (sprintf "Could not find environment %s" environmentName)

    octoProject
    |> fun p -> p.Releases
    |> Seq.choose (fun r -> r.Deployments.JsonValue.TryGetProperty environmentId)
    |> Seq.collect (fun r -> r.AsArray())
    |> Seq.tryFind (fun r -> r.["IsCurrent"].AsBoolean())
    |> function
        | Some r -> r.["ReleaseVersion"].AsString()
        | None -> failwith (sprintf "Could not find release for environment with id %s and name %s" environmentId environmentName)