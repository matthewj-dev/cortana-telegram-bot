module CortanaTelegramBot.Cache

open System
open BingChat

type TelegramToBingConvo =
    { chatId: int64
      userId: int64
      convo: BingChatConversation
      expirationEpochMs: int64 }

type BingConvoMemCache() =
    let mutable cache: TelegramToBingConvo[] = Array.empty<TelegramToBingConvo>
    let cacheSync = new Object()

    member this.GetConvo chatId userId : Option<BingChatConversation> =
        lock cacheSync (fun () ->
            let telegramToBingConvo =
                cache
                |> Array.tryFind (fun (v: TelegramToBingConvo) ->
                                       v.expirationEpochMs > DateTimeOffset.Now.ToUnixTimeMilliseconds()
                                    && v.chatId = chatId
                                    && v.userId = userId)

            match telegramToBingConvo with
            | None -> None
            | Some value -> value.convo |> Some
        )
        
    member this.RemoveConvo chatId userId: unit =
        lock cacheSync (fun () ->
            let telegramToBingConvoIndex =
                cache
                |> Array.tryFindIndex (fun (v: TelegramToBingConvo) ->
                                           v.chatId = chatId
                                        && v.userId = userId)
            let cacheWithout = match telegramToBingConvoIndex with
                               | None -> None
                               | Some indexToRemove ->cache |> Array.removeAt indexToRemove |> Some
            if cacheWithout.IsSome then
                cache <- cacheWithout.Value
            else
                ()
        )

    member this.AddConvo convo chatId userId : unit =
        lock cacheSync (fun () ->
            let rec removeDuplicates (cacheClone: TelegramToBingConvo[]) : TelegramToBingConvo[] =
                let iHuh =
                    cacheClone
                    |> Array.tryFindIndex (fun (v: TelegramToBingConvo) -> v.chatId = chatId && v.userId = userId)

                match iHuh with
                | None -> cacheClone
                | Some i ->
                    let updatedCache: TelegramToBingConvo[] = Array.removeAt i cacheClone
                    removeDuplicates updatedCache

            cache <- removeDuplicates cache

            let newTelegramToBingConvo =
                { chatId = chatId
                  userId = userId
                  convo = convo
                  expirationEpochMs = DateTimeOffset.Now.AddMinutes(60).ToUnixTimeMilliseconds() }

            cache <-
                match cache |> Array.length > 99 with
                | true -> cache |> Array.removeAt 0
                | false -> cache

            cache <- Array.append cache (Array.singleton newTelegramToBingConvo)
        )

let mutable topLevelCache = BingConvoMemCache()
