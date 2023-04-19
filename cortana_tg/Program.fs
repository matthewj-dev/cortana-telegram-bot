module cortana_tg.Program

open Funogram.Api
open Funogram.Telegram
open Funogram.Telegram.Bot


[<EntryPoint>]
let main _ =
    async {
        let config = Config.defaultConfig |> Config.withReadTokenFromFile
        let! _ = Api.deleteWebhookBase () |> api config
        return! startBot config Commands.Core.updateArrived None
    }
    |> Async.RunSynchronously

    0
