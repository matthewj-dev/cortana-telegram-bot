create table LLMConversation
(
    Conversationid        integer not null
        constraint "LLMConversation _pk"
            primary key autoincrement,
    TelegramChatId        integer not null
        constraint "LLMConversation _TelegramChat_ChatId_fk"
            references TelegramChat,
    SlidingExpirationDate integer not null
);
