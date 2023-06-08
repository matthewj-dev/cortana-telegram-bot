module CortanaTelegramBot.Program

open Funogram.Api
open Funogram.Telegram
open Funogram.Telegram.Bot
open CortanaTelegramBot.Commands.Core

[<EntryPoint>]
let main _ =
    async {
        let config = Config.withReadTokenFromEnv "TelegramBotToken" Config.defaultConfig 
        let! _ = Api.deleteWebhookBase () |> api config
        return! startBot config updateArrived None
    }
    |> Async.RunSynchronously
    
    0
