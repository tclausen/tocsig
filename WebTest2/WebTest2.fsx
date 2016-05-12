#r "packages/Suave/lib/net40/Suave.dll"
//#r "packages/MongoDB.Driver/lib/net45/MongoDB.Driver.dll"
//#r "packages/MongoDB.FSharp/lib/net40/MongoDB.FSharp.dll"
#r "packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"

//open System
//open System.Linq
//open Suave                 // always open suave
//open Suave.Successful      // for OK-result
//open Suave.Web             // for config

open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful
open Suave.RequestErrors

open Newtonsoft.Json

[<CLIMutable>]
type Todo =
  { title : string
    id : string
    ingredients: string
  }

let store = ResizeArray<Todo>()
let find id = store |> Seq.tryFind (fun t -> t.id.Equals id)
let serialize = JsonConvert.SerializeObject
let deserialize<'a> s = JsonConvert.DeserializeObject<'a> s

let getTodo id =
  match find id with
  | Some todo -> serialize todo |> OK
  | None -> NOT_FOUND id

//let todo = { todo with id = "hej" ||> sprintf "%s%O" }

let getTodo2 id =
  match find id with
  | Some todo -> serialize todo
  | None -> "Not found"

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
  //store.Add todo
  printfn "%s\n" (getTodo2 "hej")
  //startWebServer defaultConfig webPart
  0
//open MongoDB.Bson
//open MongoDB.Driver
//open MongoDB.FSharp
//Serializers.Register()
//
//type Person = { Id : BsonObjectId; Name : string; Scores : int list }
//
//let client = new MongoClient()
//let db = client.GetDatabase("test")
//
//let collection = db.GetCollection<Person> "people"
//
//let id = BsonObjectId.GenerateNewId()
//
//collection.InsertOne { Id = id; Name = "George"; Scores = [13; 52; 6] }
//
//printfn "Test"

//let query1= QueryDocument(dict ["author"==>"Kurt Vonnegut"])

//let query = new Document()
//query.["Id"] <- id
//let george2 = collection.Find(query)

//let george = collection.FindOne()
//let george = collection.find()

//printfn george2.["Name"]

//#startWebServer defaultConfig (OK "Hello World!")
