module cortana_tg.Commands.Test

open Funogram.Telegram.Bot
open Funogram.Telegram.Types
open Funogram.Telegram
open cortana_tg.Core

let testGetChatInfo (ctx: UpdateContext) config chatId =
    let msg = ctx.Update.Message.Value
    let result = botResult config (Api.getChat msg.Chat.Id)

    match result with
    | Ok x ->
        botResult config (Api.sendMessage msg.Chat.Id $"Id: %i{x.Id}, Type: {x.Type}")
        |> processResultWithValue
        |> ignore
    | Error e -> printf $"Error: %s{e.Description}"
