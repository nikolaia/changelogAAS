// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake

// Directories
let sourceDir = __SOURCE_DIRECTORY__
let buildDir  = sprintf "%s/build/" sourceDir
let deployDir = sprintf "%s/deploy/" sourceDir

type Application = private {
    Name : string
    Path : string
}
with
    static member public Create name =
        { Name = name; Path = (sprintf "%s/src/Changelog.%s/Changelog.%s.fsproj" sourceDir name name) }

let version = "0.1"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; deployDir]
)

Target "Build" <| fun _ ->
    let app =
        [ Application.Create "Suave"
          Application.Create "Provider" ]

    let appBuildDir appname = buildDir @@ sprintf "%s" appname

    app
    |> Seq.iter (fun app ->
        !! app.Path
        |> MSBuild (appBuildDir app.Name) "Build" []
        |> Log "Build-Output: " ) 

    [ "SendmailFunction"
      "AddToQueueFunction"
      "GeneratorFunction" ]
    |> Seq.iter (fun fsxFunc ->
        let dir = sprintf "%s/AzureFunctions/%s" buildDir fsxFunc
        CopyDir dir (sprintf "%s/src/AzureFunctions/%s" sourceDir fsxFunc) (fun f -> true)
        if (fsxFunc = "SendmailFunction") then
            CopyFile (sprintf "%s/CommonMark.dll" dir) "./packages/CommonMark.NET/lib/net45/CommonMark.dll"
        if (fsxFunc = "GeneratorFunction") then 
            CopyFile (sprintf "%s/FSharp.Data.dll" dir) "./packages/FSharp.Data/lib/net40/FSharp.Data.dll"
            CopyFile (sprintf "%s/FSharp.Data.DesignTime.dll" dir) "./packages/FSharp.Data/lib/net40/FSharp.Data.DesignTime.dll"
        CopyFile (sprintf "%s/Changelog.Provider.dll" dir) "./build/Provider/Changelog.Provider.dll"
        CopyFile (sprintf "%s/FSharp.Core.dll" dir) "./packages/FSharp.Core/lib/net45/FSharp.Core.dll"
        CopyFile (sprintf "%s/Types.fs" dir) "./src/Changelog.Provider/Types.fs" )

Target "Deploy" (fun _ ->
    !! (buildDir + "/**/*.*")
    -- "*.zip"
    |> Zip buildDir (deployDir + "ApplicationName." + version + ".zip")
)

// Build order
"Clean"
  ==> "Build"
  ==> "Deploy"

// start build
RunTargetOrDefault "Build"
