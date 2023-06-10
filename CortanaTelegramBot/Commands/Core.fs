module CortanaTelegramBot.Commands.Core

open Funogram.Types
open Funogram.Telegram
open Funogram.Telegram.Types
open Funogram.Telegram.Bot
open CortanaTelegramBot.Core

let getChatInfo (config: BotConfig) (chatId: int64) (messageId: int64): unit =
    let result = sendToTelegram config (Api.getChat chatId)

    match result with
    | Ok x ->
        let message = match x.Username with
                      | None -> match x.Title with
                                | None -> $"Id: %i{x.Id}, Type: {x.Type}"
                                | Some title -> $"Title: {title}, Id: %i{x.Id}, Type: {x.Type}"
                      | Some username -> $"Current User: {username}, Id: %i{x.Id}, Type: {x.Type}"
                      
        sendToTelegram config (Api.sendMessageReply chatId message messageId)
                        |> processResultWithValue
                        |> ignore
                        
    | Error e -> printf $"Error: %s{e.Description}"
    
    

let defaultText =
    """🚹🤖Available commands:
  /ask {Question} - Sends a single question to Bing.
  /get_chat_info - Returns id and type of current chat"""

let updateArrived (ctx: UpdateContext) =
    let chatId = ctx.Update.Message.Value.Chat.Id
    let userId = ctx.Update.Message.Value.From.Value.Id
    let messageId = ctx.Update.Message.Value.MessageId

    let result =
        processCommands
            ctx
            [| cmdScan "/ask %s" (fun text _ -> BingChat.askBing ctx.Config chatId userId messageId text)
               cmd "/get_chat_info" (fun _ -> getChatInfo ctx.Config chatId messageId) |]

    if result then
        Api.sendMessage chatId defaultText |> bot ctx.Config
    else
        ()