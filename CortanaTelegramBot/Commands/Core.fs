module CortanaTelegramBot.Commands.Core

open Funogram.Types
open Funogram.Telegram
open Funogram.Telegram.Types
open Funogram.Telegram.Bot
open CortanaTelegramBot.Core

let getChatInfo (config: BotConfig) (chatId: int64): unit =
    let result = botResult config (Api.getChat chatId)

    match result with
    | Ok x ->
        let message = match x.Username with
                      | None -> match x.Title with
                                | None -> $"Id: %i{x.Id}, Type: {x.Type}"
                                | Some title -> $"Title: {title}, Id: %i{x.Id}, Type: {x.Type}"
                      | Some username -> $"Current User: {username}, Id: %i{x.Id}, Type: {x.Type}"
                      
        botResult config (Api.sendMessage chatId message)
                        |> processResultWithValue
                        |> ignore
                        
    | Error e -> printf $"Error: %s{e.Description}"
    
    

let defaultText =
    """🚹🤖Available commands:
  /ask_bing {Question} - Sends a single question to Bing.
  /get_chat_info - Returns id and type of current chat"""

let updateArrived (ctx: UpdateContext) =
    let fromId () = ctx.Update.Message.Value.From.Value.Id

    let wrap fn = fn ctx.Config (fromId ())

    let result =
        processCommands
            ctx
            [| cmdScan "/ask_bing %s" (fun text _ -> BingChat.askBing ctx.Config chatId text)
               cmd "/get_chat_info" (fun _ -> getChatInfo ctx.Config chatId) |]

    if result then
        Api.sendMessage chatId defaultText |> bot ctx.Config
    else
        ()