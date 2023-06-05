namespace cortana_tg.Database.Entity

open System.ComponentModel.DataAnnotations

[<CLIMutable>]
type ChatWithLatestLLMConvo = {
    ChatId: int
    Type: string
    Name: string
    Username: Option<string>
    ConversationId: int
    SlidingExpirationDate: int
}

[<CLIMutable>]
type TelegramChat = {
    [<Key>] ChatId: option<int>
    Type: string
    Name: string
    Username: Option<string>
}

[<CLIMutable>]
type LLMConversation = {
    [<Key>] ConversationId: int
    TelegramChatId: int
    SlidingExpirationDate: int
}