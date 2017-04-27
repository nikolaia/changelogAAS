module JiraSamples

[<Literal>]
let JiraIssueSearchSample = """
{
  "expand": "names,schema",
  "startAt": 0,
  "maxResults": 50,
  "total": 1,
  "issues": [
    {
      "expand": "operations,versionedRepresentations,editmeta,changelog,transitions,renderedFields",
      "id": "14455",
      "self": "https://jira.udir.no/rest/api/2/issue/14455",
      "key": "PASX-1526",
      "fields": {
        "summary": "Se eksamensgjennomføringsdata for vgs som tar gsk eksamen",
        "issuetype": {
          "self": "https://jira.udir.no/rest/api/2/issuetype/10001",
          "id": "10001",
          "description": "gh.issue.story.desc",
          "iconUrl": "https://jira.udir.no/images/icons/issuetypes/story.svg",
          "name": "Story",
          "subtask": false
        },
        "customfield_10205": [
          {
            "self": "https://jira.udir.no/rest/api/2/customFieldOption/10131",
            "value": "Generelt",
            "id": "10131"
          }
        ],
        "fixVersions": [
          {
            "self": "https://jira.udir.no/rest/api/2/version/10249",
            "id": "10249",
            "name": "\t Gjennomføring og sensur",
            "archived": false,
            "released": false
          },
          {
            "self": "https://jira.udir.no/rest/api/2/version/10245",
            "id": "10245",
            "name": "Vår 2017",
            "archived": false,
            "released": false
          }
        ],
        "labels": ["label 1","label 2","label 3"],
        "status": {
          "self": "https://jira.udir.no/rest/api/2/status/10003",
          "description": "This status is managed internally by JIRA Software",
          "iconUrl": "https://jira.udir.no/",
          "name": "Peer review",
          "id": "10003",
          "statusCategory": {
            "self": "https://jira.udir.no/rest/api/2/statuscategory/4",
            "id": 4,
            "key": "indeterminate",
            "colorName": "yellow",
            "name": "In Progress"
          }
        }
      }
    }
  ]
}
"""