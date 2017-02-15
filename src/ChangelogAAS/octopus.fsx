module Octopus

#r "FSharp.Data"
#load "octopus.samples.fsx"

open OctopusSamples
open FSharp.Data
open FSharp.Data.HttpRequestHeaders

type OctoProjectList = JsonProvider<OctopusProjectListSample>
type OctoProject = JsonProvider<OctopusProjectSample>

let determineEnvironmentVersion projectName environmentName  =

    let octoHeader = [ "X-Octopus-ApiKey", System.Environment.GetEnvironmentVariable("OCTO_API_KEY"); Accept HttpContentTypes.Json ]

    let octoProjectListJson =
        Http.RequestString("https://oslaz-pas2-depl.udir.no/api/projects", headers = octoHeader)

    let projectId =
        (OctoProjectList.Parse octoProjectListJson).Items
        |> Seq.tryFind (fun p -> p.Name = projectName)
        |> function
            | Some p -> p.Id
            | None -> failwith (sprintf "Could not find the project %s" projectName)

    let octoProjectJson =
        let url = sprintf "https://oslaz-pas2-depl.udir.no/api/progression/%s" projectId
        Http.RequestString(url, headers = octoHeader)

    let octoProject = OctoProject.Parse octoProjectJson

    let environmentId = 
        octoProject
        |> fun p -> p.Environments
        |> Seq.tryFind (fun e -> e.Name = environmentName)
        |> function
            | Some e -> e.Id
            | None -> failwith (sprintf "Could not find environment %s" environmentName)

    OctoProject.Parse octoProjectJson
    |> fun p -> p.Releases
    |> Seq.tryFind (fun r -> match r.Deployments.JsonValue.TryGetProperty environmentId with Some _ -> true | None -> false)
    |> function
        | Some r -> r.Release.Version
        | None -> failwith (sprintf "Could not find release for environment with id %s and name %s" environmentId environmentName) 

