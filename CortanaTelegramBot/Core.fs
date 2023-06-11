module CortanaTelegramBot.Core

open System
open FSharp
open Microsoft.Extensions.Logging
open Logf
open Funogram.Types
open Funogram.Api
open BingChat

// useful for handing the entire application a top-level logging functions, no need for function passing.
let logger, exLogger as createCortanaLog: (LogLevel -> string -> unit) * (LogLevel -> string -> Exception -> unit) =
    let logger =
        LoggerFactory
            .Create(fun builder ->
                builder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Debug)
                    .AddDebug()
                    .SetMinimumLevel(LogLevel.Debug)
                    .AddFile("Logs/log.txt")
                    .SetMinimumLevel(LogLevel.Information)
                |> ignore)
            .CreateLogger()

    let cortanaLog (level: LogLevel) (message: string) : unit = logf logger level "%s{message}" message

    let cortanaLogEx (level: LogLevel) (message: string) (ex: Exception) : unit =
        elogf logger level ex "%s{message}" message

    cortanaLog, cortanaLogEx

let processResultWithValue (result: Result<'a, ApiResponseError>) =
    match result with
    | Ok v -> Some v
    | Error e ->
        printfn $"Server error: %s{e.Description}"
        None

let processResult (result: Result<'a, ApiResponseError>) = processResultWithValue result |> ignore

let sendToTelegram config data =
    api config data |> Async.RunSynchronously

let bingClient: BingChatClient =
    BingChatClientOptions(Tone = BingChatTone.Creative) |> BingChatClient

let waitFun (time: TimeSpan) =
    let timer = new Timers.Timer(time)
    let event = Async.AwaitEvent(timer.Elapsed) |> Async.Ignore
    timer.Start()

    Async.RunSynchronously event
