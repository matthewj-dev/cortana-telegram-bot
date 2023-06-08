module CortanaTelegramBot.Commands.Core

open Funogram.Telegram
open Funogram.Telegram.Types
open Funogram.Telegram.Bot
open CortanaTelegramBot.Core

let testGetChatInfo (ctx: UpdateContext) config chatId =
    let msg = ctx.Update.Message.Value
    let result = botResult config (Api.getChat msg.Chat.Id)

    match result with
    | Ok x ->
        botResult config (Api.sendMessage msg.Chat.Id $"Id: %i{x.Id}, Type: {x.Type}")
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
            [| cmdScan "/ask_bing %s" (fun text _ -> BingChat.askBing ctx ctx.Config (fromId ()) text)
               cmd "/get_chat_info" (fun _ -> testGetChatInfo ctx |> wrap) |]

    if result then
        Api.sendMessage (fromId ()) defaultText |> bot ctx.Config
    else
        ()