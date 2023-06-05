module cortana_tg.Database.CRUD

open cortana_tg.Database.Core
open cortana_tg.Database.Entity

// CREATE
let createTelegramChat (context: CortanaContext) (entity: TelegramChat): TelegramChat =
    let chat = context.TelegramChats.Add(entity)
    context.SaveChanges true |> ignore
    chat.Entity
    
let createLLMConversation (context: CortanaContext) (entity: LLMConversation): LLMConversation =
    let convo = context.LLMConversations.Add(entity)
    context.SaveChanges true |> ignore
    convo.Entity
// END CREATE

// READ
let getChat (context: CortanaContext) (chatId: int): TelegramChat option =
            query {
                for chat in context.TelegramChats do
                    where (chat.ChatId.Value = chatId)
                    select chat 
                    exactlyOne
            } |> (fun chat -> if box chat = null then None else Some chat)
            
let getLLMConversation (context: CortanaContext) (chatId: int): LLMConversation option =
            query {
                for convo in context.LLMConversations do
                    where (convo.ConversationId = chatId)
                    select convo 
                    exactlyOne
            } |> (fun chat -> if box chat = null then None else Some chat)

let getChatByIdWithLatestLLMConvo (context: CortanaContext) (chatId: int): ChatWithLatestLLMConvo option =
            query {
                for chat in context.ChatsWithLatestLLMConvo do
                    where (chat.ChatId = chatId)
                    select chat 
                    exactlyOne
            } |> (fun chat -> if box chat = null then None else Some chat)
// END READ

// UPDATE
let updateTelegramChat (context: CortanaContext) (entity: TelegramChat) =
    context.TelegramChats.Update(entity) |> ignore
    context.SaveChanges true |> ignore
    
let updateLLMConversation (context: CortanaContext) (entity: LLMConversation) =
    context.LLMConversations.Update(entity) |> ignore
    context.SaveChanges true |> ignore
// END UPDATE

// DELETE
let deleteTelegramChat (context: CortanaContext) (entity: TelegramChat) =
    context.TelegramChats.Remove(entity) |> ignore
    context.SaveChanges true |> ignore
    
let deleteLLMConversation (context: CortanaContext) (entity: LLMConversation) =
    context.LLMConversations.Remove(entity) |> ignore
    context.SaveChanges true |> ignore
// END DELETE