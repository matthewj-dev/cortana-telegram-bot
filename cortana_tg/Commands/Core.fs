module cortana_tg.Commands.Core

open Funogram.Telegram
open Funogram.Telegram.Types
open Funogram.Telegram.Bot
open cortana_tg.Core


let defaultText =
    """🚹🤖Available test commands:
  /get_chat_info - Returns id and type of current chat"""

let updateArrived (ctx: UpdateContext) =
    let fromId () = ctx.Update.Message.Value.From.Value.Id

    let wrap fn = fn ctx.Config (fromId ())

    let result =
        processCommands ctx
            [|
                cmd "/get_chat_info" (fun _ -> Test.testGetChatInfo ctx |> wrap)
            |]

    if result then
        Api.sendMessage (fromId ()) defaultText |> bot ctx.Config
    else
        ()
