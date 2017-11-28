module Teamcity

open System
open TeamcitySamples
open FSharp.Data
open FSharp.Data.HttpRequestHeaders

type TeamCityBuilds =  JsonProvider<TcBuildsSample>

let getChangeDiff buildConfigurationId newVersion oldVersion baseUrl username password =  

    let onlyMainVersions (s : string) = 
        s.Split('.')
        |> Seq.take 3
        |> String.concat "."

    let mainNewVersion = onlyMainVersions newVersion
    let mainOldVersion = onlyMainVersions oldVersion


    let sendBasicAuthRequest (url: Uri) = 
        Http.RequestString(url.ToString(), headers = [ BasicAuth username password; Accept HttpContentTypes.Json; ContentType "application/json;charset=utf-8" ])

    let relativeUrl = 
        sprintf "httpAuth/app/rest/builds?id=%s&locator=buildType:%s,sinceBuild:%s&fields=$long,build(id,number,status,changes($long,change(id,comment)))" 
            mainNewVersion 
            buildConfigurationId 
            mainOldVersion

            
    let buildsRequestUrl =  Uri (baseUrl, relativeUrl)
    
    let mapBuild (b : TeamCityBuilds.Build) =
        { Number = b.Number
          Status = b.Status
          Comments = (b.Changes.Change |> Seq.map (fun c -> c.Comment))}

    let noDots (s : string) = s.Replace(".","")

    sendBasicAuthRequest buildsRequestUrl
    |> TeamCityBuilds.Parse
    |> fun builds -> builds.Build
    |> Seq.map mapBuild
    |> Seq.filter (fun b -> 
        noDots b.Number <= noDots mainNewVersion
        && noDots b.Number >= noDots mainOldVersion)
