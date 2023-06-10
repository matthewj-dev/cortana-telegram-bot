module CortanaTelegramBot.Commands.BingChat

open System
open System.Threading.Tasks
open Funogram
open Funogram.Telegram
open Funogram.Telegram.Types
open Funogram.Tools
open Funogram.Types
open CortanaTelegramBot.Core
open Microsoft.Extensions.Logging
open Microsoft.FSharp.Control

let askBing (config: BotConfig) (chatId: int64) (inputId: int64) (input: string): unit =
    
    logger LogLevel.Information $"Processing /ask on BingChat from chatID: {chatId} with input: \n{input}"
    
    let resultMessageHuh = botResult config (Api.sendMessageReply chatId "⌛ Hold on, let me think about it." inputId)
                           |> processResultWithValue
    
    let resultMessage: Message = match resultMessageHuh with
                                    | None -> failwith "No message returned from sending message"
                                    | Some value -> value
                                    
    task {
        try
            let resultTask = bingClient.AskAsync input
            
            let mutable times = 0
            let mutable currentMessageText: string = resultMessage.Text.Value
            while resultTask.Status <> TaskStatus.RanToCompletion do
                 if times > 20 then failwith "BingChat took to long!"
                 currentMessageText <- currentMessageText + "."
                 logger LogLevel.Information "Awaiting response from BingChat"
                 500. |> TimeSpan.FromMilliseconds |> waitFun
                 botResult config (Req.EditMessageText.Make(text = currentMessageText, chatId = ChatId.Int(resultMessage.Chat.Id), messageId = resultMessage.MessageId))
                 |> processResult
                 times = times + 1 |> ignore
            
            logger LogLevel.Information "BingChat has been asked"
        
            let! result = resultTask
        
            logger LogLevel.Information "The Bing has spoken!"
        
            botResult config (Api.sendMessage chatId result)
            |> processResult
        with e ->
            exLogger LogLevel.Error $"Error asking BingChat {input}" e
            botResult config (Api.sendMessage chatId $"{e.Message} ⚠️")
            |> processResultWithValue
            |> ignore
    } |> ignore
