module cortana_tg.Commands.Core

open Funogram.Telegram
open Funogram.Telegram.Types
open Funogram.Telegram.Bot
open cortana_tg.Core


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
               cmd "/get_chat_info" (fun _ -> Test.testGetChatInfo ctx |> wrap) |]

    if result then
        Api.sendMessage (fromId ()) defaultText |> bot ctx.Config
    else
        ()
