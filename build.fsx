// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake
open PaketTemplate

// Directories
let sourceDir = __SOURCE_DIRECTORY__
let buildDir  = sprintf "%s/build/" sourceDir
let deployDir = sprintf "%s/deploy/" sourceDir

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
                                Files = [ Include ("./Changelog.Provider.dll", "/lib/dlls") ] 
                                Dependencies = [ "CommonMark.NET", AnyVersion; "FSharp.Configuration", AnyVersion ] }

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; deployDir]
)

Target "Build" <| fun _ ->
    !! app.Path
    |> MSBuildRelease (appBuildDir app.Name) "Build"
    |> Log "Build-Output: " 
    CopyFile (sprintf "%s/%s/config.yaml" buildDir app.Name) "./config/config.yaml" 

Target "Deploy" (fun _ ->
    PaketTemplate paketTemplate
    Paket.Pack (fun p -> { p with Version = version; OutputPath = "./deploy" })
)

// Build order
"Clean"
  ==> "Build"
  ==> "Deploy"

// start build
RunTargetOrDefault "Build"
