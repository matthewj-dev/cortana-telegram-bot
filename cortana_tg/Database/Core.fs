module cortana_tg.Database.Core

open Microsoft.EntityFrameworkCore
open EntityFrameworkCore.FSharp.Extensions
open cortana_tg.Database.Entity

type CortanaContext() =
    inherit DbContext()
    
    [<DefaultValue>] val mutable chatsWithLatestLLMConvo : DbSet<ChatWithLatestLLMConvo>
    member this.ChatsWithLatestLLMConvo with get() = this.chatsWithLatestLLMConvo and set value = this.chatsWithLatestLLMConvo <- value
    
    [<DefaultValue>] val mutable telegramChats : DbSet<TelegramChat>
    member this.TelegramChats with get() = this.telegramChats and set value = this.telegramChats <- value
    
    [<DefaultValue>] val mutable llmConversations : DbSet<LLMConversation>
    member this.LLMConversations with get() = this.llmConversations and set value = this.llmConversations <- value
    
    override _.OnModelCreating builder =
        builder.RegisterOptionTypes() // enables option values for all entities (for DB nulls)
        builder.Entity<ChatWithLatestLLMConvo>().ToView("ChatsWithLatestConvo_View").HasNoKey() |> ignore
        builder.Entity<TelegramChat>()
            .ToTable("TelegramChat")
            .Property(fun e -> e.ChatId).ValueGeneratedOnAdd() |> ignore
        builder.Entity<LLMConversation>()
            .ToTable("LLMConversation")
            .Property(fun e -> e.ConversationId).ValueGeneratedOnAdd |> ignore

    override __.OnConfiguring(options: DbContextOptionsBuilder) : unit =
        options.UseSqlite("Data Source=Cortana.db") |> ignore
        
let getContext () = new CortanaContext()
