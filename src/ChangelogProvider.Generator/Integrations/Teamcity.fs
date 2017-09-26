module Teamcity

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

    let sendBasicAuthRequest url = 
        Http.RequestString(url, headers = [ BasicAuth username password; Accept HttpContentTypes.Json ])

    let buildsRequestUrl = 
        sprintf "%s/httpAuth/app/rest/builds?id=%s&locator=buildType:%s,sinceBuild:%s&fields=$long,build(id,number,status,changes($long,change(id,comment)))" 
            baseUrl
            mainNewVersion 
            buildConfigurationId 
            mainOldVersion
    
    let mapBuild (b : TeamCityBuilds.Build) =
        { Number = b.Number
          Status = b.Status
          Comments = (b.Changes.Change |> Seq.map (fun c -> c.Comment))}

    let noDots (s : string) = s.Replace(".","")

    TeamCityBuilds.Parse (sendBasicAuthRequest buildsRequestUrl)
    |> fun builds -> builds.Build
    |> Seq.map mapBuild
    |> Seq.filter (fun b -> 
        noDots b.Number <= noDots mainNewVersion
        && noDots b.Number >= noDots mainOldVersion)
