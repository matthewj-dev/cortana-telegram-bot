module CortanaTelegramBot.Program

open System
open Funogram.Api
open Funogram.Telegram
open Funogram.Telegram.Bot
open CortanaTelegramBot.Commands.Core
open CortanaTelegramBot.Core
open Microsoft.Extensions.Logging
open Microsoft.FSharp.Core

[<EntryPoint>]
let main _ =
    
    let timer = new Timers.Timer(5000.)
    let event = Async.AwaitEvent (timer.Elapsed) |> Async.Ignore
    timer.Start()
    
    while true do
        async {
            try
                let config = Config.withReadTokenFromEnv "TelegramBotToken" Config.defaultConfig 
                let! _ = Api.deleteWebhookBase () |> api config
                return! startBot config updateArrived None
            with e ->
                exLogger LogLevel.Error "Error asking BingChat {input}" e
        }
        |> Async.RunSynchronously
        
        Async.RunSynchronously event
    0
