﻿module CortanaTelegramBot.Program

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

    while true do // TODO make into a service worker
        async {
            try
                let config =
                    Config.withReadTokenFromEnv "Cortana_TelegramBotToken" Config.defaultConfig

                let! _ = Api.deleteWebhookBase () |> api config
                return! startBot config updateArrived None
            with e ->
                exLogger LogLevel.Error "Unhandled Error" e
        }
        |> Async.RunSynchronously

        5. |> TimeSpan.FromSeconds |> waitFun

    0
