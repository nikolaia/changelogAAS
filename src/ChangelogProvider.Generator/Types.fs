[<AutoOpen>]
module Types

type ProjectMapping = {
    githubUrl : string
    jiraKey : string
    octoDeployName : string
    teamcityName : string
}

type ChangelogParameters = {
    projectName : string
    fromEnvironmentName : string
    toEnvironmentName : string
    octoApiKey : string
    tcUsername : string
    tcPassword : string
    jiraUsername : string
    jiraPassword : string
    octoUrl : string
    teamcityUrl : string
    jiraUrl : string
    projectMappings : ProjectMapping seq
}

type MergeCommit = {
    number : string
    message : string
    link : string
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
    Link : string
} 

type Environment = {
    Name : string
    Version : string
}

type Changelog = {
    ProjectName : string
    FromEnvironment : Environment
    ToEnvironment : Environment
    Commits : MergeCommit seq
    Issues : Issue seq
    HasDbMigration : bool
}