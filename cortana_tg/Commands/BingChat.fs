module cortana_tg.Commands.BingChat

open Funogram.Telegram.Bot
open Funogram.Telegram.Types
open Funogram.Telegram
open cortana_tg.Core

let askBing (ctx: UpdateContext) config (chatId: int64) input =
    let msg = ctx.Update.Message.Value

    task {
        let! result = bingClient.AskAsync input

        botResult config (Api.sendMessage msg.Chat.Id result)
        |> processResultWithValue
        |> ignore
    }
    |> ignore

    botResult config (Api.sendMessage msg.Chat.Id "Hold on, let me think about it...")
    |> processResultWithValue
    |> ignore
