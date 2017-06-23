#if INTERACTIVE

#I "/usr/local/lib/node_modules/azure-functions-core-tools/bin/"
#r "Microsoft.Azure.Webjobs.Host.dll"
#I "../../packages/FSharp.Interop.Dynamic/lib/portable-net45+sl50+win/"
#r "FSharp.Interop.Dynamic.dll"
#I "../../packages/Dynamitey/lib/net40/"
#r "Dynamitey.dll"
#r "System.Net.Http.dll"
#r "System.Net.Http.Formatting.dll"
#load "../Changelog.Provider/Types.fs"

#else

#load "Types.fs"

#endif

#r "Newtonsoft.Json"
#r "SendGrid"

#load "Markdown.fsx"

open System
open System.Text
open System.Linq
open System.Net
open System.Net.Http
open System.Threading.Tasks
open Microsoft.Azure.WebJobs.Host
open SendGrid.Helpers.Mail;
open Newtonsoft.Json
open FSharp.Interop.Dynamic

let Run(changelogJson: byref<obj>, log : TraceWriter, message : byref<Mail>) =

    let changelog = JsonConvert.DeserializeObject<Changelog>(string changelogJson)

    message <- Mail()   
    message.Subject <- sprintf "Changelog %s (version %s to %s)" 
                            changelog.ProjectName 
                            changelog.FromEnvironment.Version
                            changelog.ToEnvironment.Version

    let body = MarkdownToHtml.changelogToHtml changelog
        
    let content = Content("text/html", sprintf "%s" body)

    message.AddContent(content) |> ignore