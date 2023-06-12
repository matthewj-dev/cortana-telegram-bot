module CortanaTelegramBot.Commands.BingChat

open System
open System.Threading.Tasks
open CortanaTelegramBot.Cache
open Funogram
open Funogram.Telegram
open Funogram.Telegram.Types
open Funogram.Tools
open Funogram.Types
open CortanaTelegramBot.Core
open Microsoft.Extensions.Logging
open Microsoft.FSharp.Control

let askBing (config: BotConfig) (chatId: int64) (userId: int64) (inputId: int64) (input: string) : unit =
    logger LogLevel.Information $"Processing /ask on BingChat from chatID: {chatId} with input: \n{input}"

    let funLocalCache = topLevelCache

    task {
        let resultMessageHuh =
            sendToTelegram config (Api.sendMessageReply chatId "⏳" inputId)
            |> processResultWithValue

        let resultMessage: Message =
            match resultMessageHuh with
            | None -> failwith "No message returned from sending message"
            | Some value -> value
        
        try
            let! conversation =
                match funLocalCache.GetConvo chatId userId with
                | None -> bingClient.CreateConversation()
                | Some value -> Task.Run(fun _ -> value)

            funLocalCache.AddConvo conversation chatId userId
            let resultTask = conversation.AskAsync input

            logger LogLevel.Information "Awaiting response from BingChat"
            
            sendToTelegram
                config
                (Req.EditMessageText.Make(
                    text = "⏳ Hold on, let me think about it...",
                    chatId = ChatId.Int(resultMessage.Chat.Id),
                    messageId = resultMessage.MessageId
                ))
            |> processResult

            let! result = resultTask

            logger LogLevel.Information "The Bing has spoken!"
            
            if result.Trim().ToLowerInvariant().Contains "<disengaged>" then
                funLocalCache.RemoveConvo chatId userId

            sendToTelegram
                config
                (Req.EditMessageText.Make(
                    text = "⌛",
                    chatId = ChatId.Int(resultMessage.Chat.Id),
                    messageId = resultMessage.MessageId
                ))
            |> processResult

            0.5 |> TimeSpan.FromSeconds |> waitFun
            
            sendToTelegram
                config
                (Req.EditMessageText.Make(
                    text = "✅",
                    chatId = ChatId.Int(resultMessage.Chat.Id),
                    messageId = resultMessage.MessageId
                ))
            |> processResult

            sendToTelegram
                config
                (Req.EditMessageText.Make(
                    text = result,
                    chatId = ChatId.Int(resultMessage.Chat.Id),
                    messageId = resultMessage.MessageId
                ))
            |> processResult
        with e ->
            exLogger LogLevel.Error $"Error asking BingChat {input}" e

            sendToTelegram
                config
                (Req.EditMessageText.Make(
                    text = "❌",
                    chatId = ChatId.Int(resultMessage.Chat.Id),
                    messageId = resultMessage.MessageId
                ))
            |> processResult

            1. |> TimeSpan.FromSeconds |> waitFun

            sendToTelegram
                config
                (Req.DeleteMessage.Make(chatId = ChatId.Int(resultMessage.Chat.Id), messageId = resultMessage.MessageId))
            |> processResult
    }
    |> ignore

    topLevelCache <- funLocalCache
