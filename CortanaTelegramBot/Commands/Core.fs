﻿module CortanaTelegramBot.Commands.Core

open Funogram.Types
open Funogram.Telegram
open Funogram.Telegram.Types
open Funogram.Telegram.Bot
open CortanaTelegramBot.Core
open Microsoft.Extensions.Logging

let getChatInfo (config: BotConfig) (chatId: int64) (messageId: int64) : unit =
    let result = sendToTelegram config (Api.getChat chatId)

    match result with
    | Ok x ->
        let message =
            match x.Username with
            | None ->
                match x.Title with
                | None -> $"Id: %i{x.Id}, Type: {x.Type}"
                | Some title -> $"Title: {title}, Id: %i{x.Id}, Type: {x.Type}"
            | Some username -> $"Current User: {username}, Id: %i{x.Id}, Type: {x.Type}"

        sendToTelegram config (Api.sendMessageReply chatId message messageId)
        |> processResultWithValue
        |> ignore

    | Error e -> logger LogLevel.Error $"Error: %s{e.Description}"



let defaultText =
    """🚹🤖Available commands:
  /ask {Question} - Sends a single question to Bing.
  /chat_info - Returns id and type of current chat"""

let updateArrived (ctx: UpdateContext) =
    match ctx.Update.Message with
    | None -> ()
    | Some message ->
        let chatId = message.Chat.Id
        let userId = message.From.Value.Id
        let messageId = message.MessageId

        processCommands
            ctx
            [| cmdScan "/ask %s" (fun text _ -> BingChat.askBing ctx.Config chatId userId messageId text)
               cmd "/chat_info" (fun _ -> getChatInfo ctx.Config chatId messageId)
               cmd "/help" (fun _ -> Api.sendMessage chatId defaultText |> sendToTelegram ctx.Config |> ignore) |]
        |> ignore
        