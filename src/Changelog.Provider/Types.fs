[<AutoOpen>]
module Types

type ChangelogParameters = {
    ProjectName : string
    fromEnvironment : string
    toEnvironment : string
    octoApiKey : string
    octoBaseUrl : string
    tcBaseUrl : string
    tcUsername : string
    tcPassword : string
    jiraBaseUrl : string
    jiraUsername : string
    jiraPassword : string
}

type MergeCommit = {
    number : string
    message : string
    link : string
}

type Issue = {
    Key : string
    Summary : string
    Issuetype : string
    FixVersions : string
    ApplicationUser: string
    Labels : string
    Status : string
    Link : string
}

type Changelog = {
    fromVersion: string
    toVersion: string
    Commits : MergeCommit seq
    Issues : Issue seq
    HasDbMigration : bool
}