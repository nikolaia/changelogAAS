module Teamcity

open TeamcitySamples
open FSharp.Data
open FSharp.Data.HttpRequestHeaders

type TeamCityBuilds =  JsonProvider<TcBuildsSample>

let getChangeDiff projectName newVersion oldVersion baseUrl username password =  

    let sendBasicAuthRequest url = 
        Http.RequestString(url, headers = [ BasicAuth username password; Accept HttpContentTypes.Json ])

    (*  The mapping between projectName and build configuration ID has been
        hardcoded because that was the easiest thing to do to get things
        working. If you have a smart solution for how to get this more generic
        please let me know! :) *) 
    let buildConfigurationId = 
        match projectName with
        | "UDIR.Prover" -> "Prover_ReleaseProverHoved"
        | "UDIR.PAS2" -> "Pas2_ReleasePas2HovedPsake"
        | "UDIR.PAS2.Infrastructure" -> "Pas2_CiPas2infrastructure"
        | "UDIR.ID" -> "Pas2_ReleasePas2idExperimental"
        | _ -> failwith (sprintf "Could not map projectname %s to a TeamCity buildType" projectName)

    let buildsRequestUrl = 
        sprintf "%s/httpAuth/app/rest/builds?id=%s&locator=buildType:%s,sinceBuild:%s&fields=$long,build(id,number,status,changes($long,change(id,comment)))" 
            baseUrl
            newVersion 
            buildConfigurationId 
            oldVersion

    TeamCityBuilds.Parse (sendBasicAuthRequest buildsRequestUrl)
    |> fun builds -> builds.Build
    |> Seq.map (fun b -> b.Number, b.Status, b.Changes.Change)
