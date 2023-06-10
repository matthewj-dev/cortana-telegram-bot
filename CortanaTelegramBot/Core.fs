module CortanaTelegramBot.Core

open System
open FSharp
open Microsoft.Extensions.Logging
open Logf
open Funogram.Types
open Funogram.Api
open BingChat

let processResultWithValue (result: Result<'a, ApiResponseError>) =
    match result with
    | Ok v -> Some v
    | Error e ->
        printfn $"Server error: %s{e.Description}"
        None

let processResult (result: Result<'a, ApiResponseError>) = processResultWithValue result |> ignore

let botResult config data =
    api config data |> Async.RunSynchronously

let bot config data = botResult config data |> processResult

let bingClient: BingChatClient =
    BingChatClientOptions(Cookie = Environment.GetEnvironmentVariable "BingToken")
    |> BingChatClient

// useful for handing the entire application a top-level logging functions, no need for function passing.
let logger, exLogger as createCortanaLog: (LogLevel -> string -> unit) * (LogLevel -> string -> Exception -> unit) =
    let logger = LoggerFactory.Create(fun builder -> builder
                                                         .AddConsole().SetMinimumLevel(LogLevel.Information)
                                                         .AddDebug().SetMinimumLevel(LogLevel.Debug) |> ignore)
                                                         .CreateLogger()
    let cortanaLog (level: LogLevel) (message: string): unit =
        logf logger level "%s{message}" message
    let cortanaLogEx (level: LogLevel) (message: string) (ex: Exception): unit =
        elogf logger level ex "%s{message}" message
    cortanaLog, cortanaLogEx