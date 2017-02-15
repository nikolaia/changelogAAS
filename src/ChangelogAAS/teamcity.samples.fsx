module TeamcitySamples

[<Literal>]
let TcBuildsSample = """
{
  "count": 4,
  "href": "/httpAuth/app/rest/builds?id=1.0.120&locator=buildType:Prover_ReleaseProverHoved,sinceBuild:1.0.117&fields=$long,build(id,number,status,changes($long,change(id,comment)))",
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
            "comment": "Merge pull request #279 from Utdanningsdirektoratet/b_vask_fake_skoledata\n\nvask fake data for fake eksamenstjenesteclient"
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
            "comment": "Merge pull request #274 from Utdanningsdirektoratet/b_fix_web_smoke_for_sas_env\n\nSet correct resource address and scope for ID-api in SAS, PTWOUTV-5060"
          },
          {
            "id": 61668,
            "comment": "Set correct resource address and scope for ID-api in SAS\n"
          }
        ]
      }
    }
  ]
}
"""
