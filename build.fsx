// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake
open PaketTemplate

// Directories
let sourceDir = __SOURCE_DIRECTORY__
let buildDir  = sprintf "%s/build/" sourceDir
let artifactDir = sprintf "%s/artifacts/" sourceDir

type Application = private {
    Name : string
    Path : string
    BuildDir : string
}
with
    static member public Create name =
        { Name = name; 
          Path = (sprintf "%s/src/Changelog.%s/Changelog.%s.fsproj" sourceDir name name)
          BuildDir = (sprintf "%s/%s/" buildDir name) }

let version =
    match buildServer with
    | TeamCity | AppVeyor -> buildVersion
    | _ -> environVarOrDefault "version" "1.0.0"

let app = Application.Create "Provider"

let appBuildDir appname = buildDir @@ sprintf "%s" appname

let paketTemplate p =  { p with TemplateFilePath = Some (sprintf "%s/%s/paket.template" buildDir app.Name)
                                TemplateType = File
                                Id = Some "ChangelogAAS.Provider" 
                                Version = Some version
                                Description = ["Changelog Provider Library"]
                                Authors = ["Idéhub AS"] 
                                Files = [ Include ("./Changelog.Provider.dll", "/lib/net45") ] 
                                Dependencies = [ "CommonMark.NET", GreaterOrEqual <| Version "0.15.1"; 
                                                 "FSharp.Configuration", GreaterOrEqual <| Version "1.3.0"
                                                 "SharpYaml", GreaterOrEqual <| Version "1.6.1"
                                                 "FSharp.Data", GreaterOrEqual <| Version "2.3.3" ] }

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; artifactDir]
)

Target "Build" <| fun _ ->
    !! app.Path
    |> MSBuildRelease (appBuildDir app.Name) "Build"
    |> Log "Build-Output: "

Target "Pack" (fun _ ->
    PaketTemplate paketTemplate
    Paket.Pack (fun p -> { p with Version = version; OutputPath = "./artifacts" })
)

// Build order
"Clean"
  ==> "Build"
  ==> "Pack"

// start build
RunTargetOrDefault "Build"
