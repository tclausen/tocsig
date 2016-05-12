open SuaveRestApi.Db
open SuaveRestApi.DbRecipe
open SuaveRestApi.Rest
open Suave
open Suave.Successful
open Suave.Filters
open Suave.RequestErrors
open Suave.Operators
open Suave.Web
open Suave.Writers
open Suave.WebPart
open System.Net

let testStorage fileName =
  DbRecipe.myInit
//  let newRecipe = {
//    MyId = 3
//    Title = "Sovs 2"
//    Process = "hej"
//    Ingredients = "hej2"
//    Source = "mig"
//  }
//  DbRecipe.createRecipe newRecipe |> ignore
  printfn "%s" (DbRecipe.getRecipes() |> DbRecipe.toJson)
  ()

[<EntryPoint>]
let main argv =

  let config = 
    { defaultConfig with
        bindings =
          [ HttpBinding.mk HTTP (IPAddress.Parse "192.168.1.70") 8083us ]
    }

  let myResp =
      //printfn "Got a post request"
      OK "Hello POST"

  let app =
    choose
      [ GET >=> choose
          [ path "/recipes" >=> OK "Hello GET"
            path "/goodbye" >=> OK "Good bye GET" ]
        POST >=> choose
          [ path "/recipes" >=> myResp
            path "/goodbye" >=> OK "Good bye POST" ] ]


  let recipeWebPart = rest "recipes" {
    GetAll = DbRecipe.getRecipes
    GetById = DbRecipe.getRecipe
    Create = DbRecipe.createRecipe
    Update = DbRecipe.updateRecipe
    Delete = DbRecipe.deleteRecipe
    UpdateById = DbRecipe.updateRecipeById
    IsExists = DbRecipe.isRecipeExists
  }

  let greetings q =
    printfn "%A" q
    "hello"
    
  let recipeWithOptionWebPart : WebPart = 
    choose [
      recipeWebPart
        >=> setHeader "Access-Control-Allow-Origin" "*"
        >=> setHeader "Access-Control-Allow-Methods" "GET, PUT, DELETE, POST"
        >=> setHeader "Access-Control-Allow-Headers" "Origin, Content-Type, X-Auth-Token"
      OPTIONS >=> request(fun r -> OK <| greetings r.query)
        >=> setHeader "Access-Control-Allow-Origin" "*"
        >=> setHeader "Access-Control-Allow-Methods" "GET, PUT, DELETE, POST"
        >=> setHeader "Access-Control-Allow-Headers" "Origin, Content-Type, X-Auth-Token"
    ]

  //testStorage "hej"
  //startWebServer config recipeWebPart
  startWebServer config recipeWithOptionWebPart
  0
