module TeamcitySamples

[<Literal>]
let TcBuildsSample = """
{
  "count": 4,
  "href": "/httpAuth/app/rest/builds?id=1.0.120&locator=buildType:Example1_BuildAndRelease,sinceBuild:1.0.117&fields=$long,build(id,number,status,changes($long,change(id,comment)))",
  "build": [
    {
      "id": 24475,
      "number": "1.0.121",
      "status": "SUCCESS",
      "changes": {
        "count": 12,
        "href": "/httpAuth/app/rest/changes?locator=build:(id:24475)",
        "change": [
          {
            "id": 61699,
            "comment": "Merge pull request #279 from github/example1\n\nSomething something whatever"
          }
        ]
      }
    },
    {
      "id": 24457,
      "number": "1.0.120",
      "status": "SUCCESS",
      "changes": {
        "count": 2,
        "href": "/httpAuth/app/rest/changes?locator=build:(id:24457)",
        "change": [
          {
            "id": 61669,
            "comment": "Merge pull request #278 from github/example1\n\nSomething something whatever"
          },
          {
            "id": 61668,
            "comment": "Merge pull request #277 from github/example1\n\nSomething something whatever"
          }
        ]
      }
    }
  ]
}
"""
