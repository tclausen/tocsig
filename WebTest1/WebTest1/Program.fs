module tocsig.App

open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful
open Suave.RequestErrors

let webPartHelloWorld = path "/" >=> Successful.OK "Hello World"

let browseRecipes =
    request (fun r ->
        match r.queryParam "type" with
        | Choice1Of2 genre -> OK (sprintf "Type: %s" genre)
        | Choice2Of2 msg -> BAD_REQUEST msg)

let webPart = 
    choose [
        path "/" >=> (Successful.OK "Home")
        path "/recipes" >=> browseRecipes
        pathScan "/recipes/details/%d" (fun id -> Successful.OK (sprintf "Recipe details: %d" id))
        Successful.OK "404. Not found" 
    ]

[<EntryPoint>]
let main argv = 
    startWebServer defaultConfig webPart
    0