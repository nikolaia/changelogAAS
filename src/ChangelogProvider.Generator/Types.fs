[<AutoOpen>]
module Types

open System

type ProjectMapping = {
    githubUrl : Uri
    jiraKey : string
    octoDeployName : string
    teamcityName : string
}

type EnvironmentChangelogParameters = {
    projectName : string
    fromEnvironmentName : string
    toEnvironmentName : string
    octoApiKey : string
    octoUrl : Uri

    tcUsername : string
    tcPassword : string
    jiraUsername : string
    jiraPassword : string
    teamcityUrl : Uri
    jiraUrl : Uri
    projectMappings : ProjectMapping seq
} 
    

type ChangelogParameters = {
    projectName : string
    fromVersion : string
    toVersion : string
    
    tcUsername : string
    tcPassword : string
    jiraUsername : string
    jiraPassword : string
    teamcityUrl : Uri
    jiraUrl : Uri
    projectMappings : ProjectMapping seq
}

type MergeCommit = {
    number : string
    message : string
    link : Uri
}

type IssueType =
    | Bug
    | Story
    | Task
    | Unknown of string
with 
    static member FromString s = 
        match s with
         | "Bug" -> Bug
         | "Story" -> Story
         | "Task" -> Task
         | s -> Unknown s
    static member ToString s = 
        match s with
         | Bug -> "Bug"
         | Story -> "Story"
         | Task -> "Task"
         | Unknown s -> s
    static member GetColor it =
        match it with
        | Bug -> "#CC0000"
        | Story -> "#8D6811"
        | Task -> "#8DC9CB"
        | _ -> "#333333" 

type Build = {
    Number : string
    Status : string
    Comments : string seq
}

type Issue = {
    Key : string
    Summary : string
    Issuetype : IssueType
    FixVersions : string
    ApplicationUser: string
    Labels : string
    Status : string
    Link : Uri
} 

type Changelog = {
    ProjectName : string
    FromVersion : string
    ToVersion : string
    Commits : MergeCommit seq
    Issues : Issue seq
    HasDbMigration : bool
}