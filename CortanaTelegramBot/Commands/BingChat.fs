module CortanaTelegramBot.Commands.BingChat

open Funogram.Telegram.Bot
open Funogram.Telegram.Types
open Funogram.Telegram
open Funogram.Types
open CortanaTelegramBot.Core

let askBing (ctx: UpdateContext) (config: BotConfig) (chatId: int64) (input: string): unit =
    if ctx.Update.Message.IsNone then
        ()
    
    let msg = ctx.Update.Message.Value

    task {
        let! conversation = bingClient.CreateConversation()
        let! result = conversation.AskAsync input
        
        // TODO this library can not give back a conversation ID...
        // TODO might have to maintain a singleton collection of all recent chats -> convos... so much for database...

        botResult config (Api.sendMessage msg.Chat.Id result)
        |> processResultWithValue
        |> ignore
    }
    |> ignore

    botResult config (Api.sendMessage msg.Chat.Id "Hold on, let me think about it...")
    |> processResultWithValue
    |> ignore
