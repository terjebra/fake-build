#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open System
open System.IO

let version = "1.0.0";

let docker args =
    printfn "%s" args
    let dockerCommand = if Environment.isWindows then "docker.exe" else "docker";
    let result =
        Process.execWithResult (fun (info:ProcStartInfo) ->
          { info with
                FileName = dockerCommand
                Arguments = args 
          })
          (System.TimeSpan.FromMinutes 15.)
    
    if result.ExitCode <> 0 then
        let errors = String.Join(Environment.NewLine,result.Errors)
        printfn "%s" <| String.Join(Environment.NewLine,result.Messages)
        failwithf "FAKE Process exited with %d: %s" result.ExitCode errors
    0 |> ignore;

Target.create  "BuildDockerImage" (fun _ ->
    let id = System.Guid.NewGuid().ToString()
    docker (sprintf "build . -t app-%s" id)
    docker (sprintf "tag app-%s app:latest" id)
    )
    

Target.runOrDefault "BuildDockerImage"
