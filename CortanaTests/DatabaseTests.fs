module DatabaseTests

open System
open Xunit
open cortana_tg.Database.Entity
open cortana_tg.Database.Core
open cortana_tg.Database.CRUD

// can't or at least don't have time to figure out how to mock sqlite db, so real data is all ur gonna get

let rnd = Random()
let context = getContext()

[<Fact>]
let ``Create a telegram chat`` () =
    let expectedChat: TelegramChat = {
                                       ChatId = option.None
                                       Type = "private"
                                       Name = "BLA"
                                       Username = "@username" |> option.Some
                                       }
    let expectedChat1: TelegramChat = {
                                   ChatId = option.None
                                   Type = "private"
                                   Name = "BLA"
                                   Username = "@username" |> option.Some
                                   }
    let expectedChat2: TelegramChat = {
                               ChatId = option.None
                               Type = "private"
                               Name = "BLA"
                               Username = "@username" |> option.Some
                               }
    
    let createdChat: TelegramChat = createTelegramChat context expectedChat
    
    (expectedChat, createdChat) |> Assert.Equivalent

[<Fact>]    
let ``Create a LLM convo`` () =
    let expectedChat: TelegramChat = {
                                   ChatId = option.None
                                   Type = "private"
                                   Name = ""
                                   Username = "@username" |> option.Some
                                   }
    let createdChat: TelegramChat = createTelegramChat context expectedChat
    
    let expectedConvo: LLMConversation = { ConversationId = rnd.Next()
                                           TelegramChatId = createdChat.ChatId.Value
                                           SlidingExpirationDate = rnd.Next()
                                           }
    
    
    
    let createdConvo: LLMConversation = createLLMConversation context expectedConvo
    
    (expectedConvo, createdConvo) |> Assert.Equivalent
    