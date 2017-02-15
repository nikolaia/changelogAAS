module OctopusSamples

[<Literal>]
let OctopusProjectListSample = """
{
"ItemType": "Project",
"IsStale": false,
"TotalResults": 13,
"ItemsPerPage": 30,
"Items": [
    {
    "Id": "projects-161",
    "VariableSetId": "variableset-projects-161",
    "DeploymentProcessId": "deploymentprocess-projects-161",
    "IncludedLibraryVariableSetIds": [],
    "DefaultToSkipIfAlreadyInstalled": false,
    "VersioningStrategy": {
        "DonorPackageStepId": null,
        "Template": "#{Octopus.Version.LastMajor}.#{Octopus.Version.LastMinor}.#{Octopus.Version.NextPatch}"
    },
    "ReleaseCreationStrategy": {
        "ReleaseCreationPackageStepId": ""
    },
    "Name": "Certificates",
    "Slug": "certificates",
    "Description": "",
    "IsDisabled": false,
    "ProjectGroupId": "ProjectGroups-33",
    "LifecycleId": "lifecycles-97",
    "AutoCreateRelease": false,
    "LastModifiedOn": "2015-12-10T12:49:05.153+00:00",
    "LastModifiedBy": "nan@ls.no",
    "Links": {
        "Self": "/api/projects/projects-161",
        "Releases": "/api/projects/projects-161/releases{/version}{?skip}",
        "Variables": "/api/variables/variableset-projects-161",
        "Progression": "/api/progression/projects-161",
        "DeploymentProcess": "/api/deploymentprocesses/deploymentprocess-projects-161",
        "Web": "/app#/projects/projects-161",
        "Logo": "/api/projects/projects-161/logo"
    }
    }
]
}
"""

[<Literal>]
let OctopusProjectSample = """
{
    "Environments": [
    {
        "Id": "Environments-97",
        "Name": "PAS2-DEV"
    }
    ],
    "Releases": [
    {
        "Release": {
        "Id": "releases-4492",
        "Assembled": "2016-09-28T11:35:00.340+00:00",
        "ReleaseNotes": null,
        "ProjectId": "projects-481",
        "ProjectVariableSetSnapshotId": "variableset-projects-481-snapshot-13",
        "LibraryVariableSetSnapshotIds": [],
        "ProjectDeploymentProcessSnapshotId": "deploymentprocess-projects-481-snapshot-5",
        "SelectedPackages": [
            {
            "StepName": "Deploy web HTTPS",
            "Version": "1.0.105"
            },
            {
            "StepName": "Deploy web HTTP",
            "Version": "1.0.105"
            },
            {
            "StepName": "Migrate database",
            "Version": "1.0.104"
            },
            {
            "StepName": "Deploy web API HTTPS",
            "Version": "1.0.104"
            },
            {
            "StepName": "Deploy web API HTTP",
            "Version": "1.0.104"
            }
        ],
        "Version": "1.0.105",
        "Links": {
            "Self": "/api/releases/releases-4492",
            "Project": "/api/projects/projects-481",
            "Progression": "/api/releases/releases-4492/progression",
            "Deployments": "/api/releases/releases-4492/deployments",
            "DeploymentTemplate": "/api/releases/releases-4492/deployments/template",
            "Artifacts": "/api/artifacts?regarding=releases-4492",
            "ProjectVariableSnapshot": "/api/variables/variableset-projects-481-snapshot-13",
            "ProjectDeploymentProcessSnapshot": "/api/deploymentprocesses/deploymentprocess-projects-481-snapshot-5",
            "Web": "/app#/releases/releases-4492",
            "SnapshotVariables": "/api/releases/releases-4492/snapshot-variables",
            "Defects": "/api/releases/releases-4492/defects",
            "ReportDefect": "/api/releases/releases-4492/defects",
            "ResolveDefect": "/api/releases/releases-4492/defects/resolve"
        }
        },
        "Deployments": {
        "Environments-97": {
            "Id": "deployments-8992",
            "ProjectId": "projects-481",
            "EnvironmentId": "Environments-97",
            "ReleaseId": "releases-4492",
            "DeploymentId": "deployments-8992",
            "TaskId": "ServerTasks-32731",
            "ReleaseVersion": "1.0.105",
            "Created": "2016-09-28T11:35:20.183+00:00",
            "QueueTime": "2016-09-28T11:35:20.183+00:00",
            "CompletedTime": "2016-09-28T11:36:56.496+00:00",
            "State": "Success",
            "HasPendingInterruptions": false,
            "HasWarningsOrErrors": false,
            "ErrorMessage": "",
            "Duration": "2 minutes",
            "IsCurrent": true,
            "IsPrevious": false,
            "Links": {
            "Self": "/api/deployments/deployments-8992",
            "Release": "/api/releases/releases-4492",
            "Task": "/api/tasks/ServerTasks-32731"
            }
        }
        },
        "NextDeployments": [
        "Environments-33",
        "Environments-129",
        "Environments-65"
        ],
        "HasUnresolvedDefect": false,
        "ReleaseRetentionPeriod": null,
        "TentacleRetentionPeriod": null
    }
    ]
}
"""
