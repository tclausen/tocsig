﻿namespace SuaveRestApi.DbRecipe

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open System.Collections.Generic
open System
open System.Text

type Recipe = {
  MyId : int
  Title : string
  Ingredients : string
  Source : string
  Process : string
  Created : string
  Tags : string
  Category : string
}

module DbRecipe =
  let private recipeStorage = new Dictionary<int, Recipe>()
  let timeNow = System.DateTime.Now.ToLongTimeString()
  let timeNowUpToDate () = System.DateTime.Now

  // 'a -> string
  let toJson v =
    let jsonSerializerSettings = new JsonSerializerSettings()
    jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
    JsonConvert.SerializeObject(v, jsonSerializerSettings)

  // string -> 'a 
  let fromJson<'a> json =
    JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a

  let getRecipes () =
    recipeStorage.Values |> Seq.map (fun p -> p)

  let fileName = "DbRecipes.txt"
  
  let writeToFile o =
    printfn "Writing to %s" fileName
    use w = new System.IO.StreamWriter(fileName)
    w.Write(toJson o)
    w.Close()

  let readFromFile (fn : string) : Dictionary<int, Recipe> =
    use w = new System.IO.StreamReader(fn)
    let t = w.ReadLine() 
    t |> fromJson<Dictionary<int, Recipe>>

  let myInit =
    recipeStorage.Clear()  
    for KeyValue(k,v) in readFromFile fileName do
        recipeStorage.Add(k,v)

  let getRecipe id =
    if recipeStorage.ContainsKey(id) then
      Some recipeStorage.[id]
    else
      None

  let rec getUniqueKey key =
    if recipeStorage.ContainsKey key then
      getUniqueKey (key + 1)
    else
      key

  let createRecipe recipe =
    let id =
      getUniqueKey (recipeStorage.Values.Count + 1)
    printfn "Create recipe with id = %i" id
    let newRecipe = {
      MyId = id
      Process = recipe.Process.Replace(Environment.NewLine, ";")
      Title = recipe.Title
      Source = recipe.Source
      Ingredients = recipe.Ingredients.Replace(Environment.NewLine, ";")
      Created = timeNowUpToDate().ToString("dd-MM-yyyy HH:mm")
      Tags = recipe.Tags
      Category = recipe.Category
    }
    recipeStorage.Add(id, newRecipe)
    recipeStorage |> writeToFile 
    newRecipe  

  let updateRecipeById recipeId recipeToBeUpdated =
    printfn "Update recipe %i" recipeId
    if recipeStorage.ContainsKey(recipeId) then
      let updatedRecipe = {
        MyId = recipeId
        Process = recipeToBeUpdated.Process
        Title = recipeToBeUpdated.Title
        Source = recipeToBeUpdated.Source
        Ingredients = recipeToBeUpdated.Ingredients
        Created = recipeToBeUpdated.Created
        Tags = recipeToBeUpdated.Tags
        Category = recipeToBeUpdated.Category
      }
      recipeStorage.[recipeId] <- updatedRecipe
      recipeStorage |> writeToFile 
      Some updatedRecipe
    else
      None

  let updateRecipe recipeToBeUpdated =
    updateRecipeById recipeToBeUpdated.MyId recipeToBeUpdated

  let deleteRecipe recipeId =
    printfn "Delete recipe %i" recipeId
    recipeStorage.Remove(recipeId) |> ignore
    recipeStorage |> writeToFile 

  let isRecipeExists = recipeStorage.ContainsKey


