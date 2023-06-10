module CortanaTelegramBot.Commands.BingChat

open Funogram.Telegram
open Funogram.Types
open CortanaTelegramBot.Core
open Microsoft.Extensions.Logging

let askBing (config: BotConfig) (chatId: int64) (input: string): unit =
    
    logger LogLevel.Information $"Processing /ask_bing from chatID: {chatId} with input: \n{input}"
    
    task {
        try
            let! conversation = bingClient.CreateConversation()
            let! result = conversation.AskAsync input
            
            botResult config (Api.sendMessage chatId result)
            |> processResultWithValue
            |> ignore
        with e ->
            exLogger LogLevel.Error "Error asking BingChat {input}" e
            botResult config (Api.sendMessage chatId $"{e.Message} ⚠️")
            |> processResultWithValue
            |> ignore
    }
    |> ignore
    
    logger LogLevel.Information "BingChat has been asked"

    botResult config (Api.sendMessage chatId "Hold on, let me think about it... ⌛")
    |> processResultWithValue
    |> ignore
