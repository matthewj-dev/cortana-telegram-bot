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

    let funLocalCache = topLevelCache // capture cache instance to prevent deadlocks TODO make the cache use AsyncSeq

    task {
        let resultMessageHuh =
            sendToTelegram config (Api.sendMessageReply chatId "⏳ Hold on, let me think about it." inputId)
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

            funLocalCache.AddConvo conversation chatId userId |> ignore
            let resultTask = conversation.AskAsync input

            logger LogLevel.Information "Awaiting response from BingChat"

            let mutable times: int = 0
            let mutable currentMessageText: string = resultMessage.Text.Value

            while resultTask.Status <> TaskStatus.RanToCompletion do
                if times > 60 then
                    failwith "BingChat took to long!"

                1. |> TimeSpan.FromSeconds |> waitFun

                currentMessageText <- currentMessageText + "."

                sendToTelegram
                    config
                    (Req.EditMessageText.Make(
                        text = currentMessageText,
                        chatId = ChatId.Int(resultMessage.Chat.Id),
                        messageId = resultMessage.MessageId
                    ))
                |> processResult

                times <- times + 1

            logger LogLevel.Information "BingChat has been asked"

            let! result = resultTask

            logger LogLevel.Information "The Bing has spoken!"

            sendToTelegram config (Api.sendMessage chatId result) |> processResult

            sendToTelegram
                config
                (Req.EditMessageText.Make(
                    text = "⌛",
                    chatId = ChatId.Int(resultMessage.Chat.Id),
                    messageId = resultMessage.MessageId
                ))
            |> processResult

            1. |> TimeSpan.FromSeconds |> waitFun

            sendToTelegram
                config
                (Req.DeleteMessage.Make(chatId = ChatId.Int(resultMessage.Chat.Id), messageId = resultMessage.MessageId))
            |> processResult
        with e ->
            exLogger LogLevel.Error $"Error asking BingChat {input}" e

            sendToTelegram
                config
                (Req.EditMessageText.Make(
                    text = "💀",
                    chatId = ChatId.Int(resultMessage.Chat.Id),
                    messageId = resultMessage.MessageId
                ))
            |> processResult

            1. |> TimeSpan.FromSeconds |> waitFun

            sendToTelegram
                config
                (Req.DeleteMessage.Make(chatId = ChatId.Int(resultMessage.Chat.Id), messageId = resultMessage.MessageId))
            |> processResult

            sendToTelegram config (Api.sendMessage chatId $"{e.Message} ⚠️")
            |> processResultWithValue
            |> ignore
    }
    |> ignore
