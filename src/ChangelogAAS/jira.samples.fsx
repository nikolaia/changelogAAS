module JiraSamples

[<Literal>]
let JiraIssueSearchSample = """
{                                                                                                                                                                                   
  "expand": "names,schema",                                                                                                                                                           
  "startAt": 0,                                                                                                                                                                       
  "maxResults": 2,                                                                                                                                                                    
  "total": 1,                                                                                                                                                                         
  "issues": [                                                                                                                                                                         
    {                                                                                                                                                                                 
      "expand": "editmeta,renderedFields,transitions,changelog,operations",                                                                                                           
      "id": "71749",                                                                                                                                                                  
      "self": "https://pashjelp.udir.no/rest/api/2/issue/71749",                                                                                                                      
      "key": "PTWOUTV-5059",                                                                                                                                                          
      "fields": {                                                                                                                                                                     
        "summary": "Klagekarakterer, inntakskontor",                                                                                                                                  
        "customfield_12122": {                                                                                                                                                        
          "self": "https://pashjelp.udir.no/rest/api/2/customFieldOption/11100",                                                                                                      
          "value": "Oppnevning",                                                                                                                                                      
          "id": "11100"                                                                                                                                                               
        },                                                                                                                                                                            
        "issuetype": {                                                                                                                                                                
          "self": "https://pashjelp.udir.no/rest/api/2/issuetype/38",                                                                                                                 
          "id": "38",                                                                                                                                                                 
          "description": "A problem which impairs or prevents the functions of the product.",                                                                                         
          "iconUrl": "https://pashjelp.udir.no/images/icons/issuetypes/bug.png",                                                                                                      
          "name": "Bug",                                                                                                                                                              
          "subtask": false                                                                                                                                                            
        },                                                                                                                                                                            
        "status": {                                                                                                                                                                   
          "self": "https://pashjelp.udir.no/rest/api/2/status/10023",                                                                                                                 
          "description": "This status is managed internally by JIRA Agile",                                                                                                           
          "iconUrl": "https://pashjelp.udir.no/images/icons/subtask.gif",                                                                                                             
          "name": "I test",                                                                                                                                                           
          "id": "10023"                                                                                                                                                               
        },                                                                                                                                                                            
        "customfield_12121": {                                                                                                                                                        
          "self": "https://pashjelp.udir.no/rest/api/2/customFieldOption/10600",                                                                                                      
          "value": "S/S",                                                                                                                                                             
          "id": "10600"                                                                                                                                                               
        }                                                                                                                                                                             
      }                                                                                                                                                                               
    }                                                                                                                                                                                 
  ]                                                                                                                                                                                   
}
"""