create view ChatsWithLatestConvo_View as
select
    TC.ChatId
     ,TC.Type
     ,TC.Name
     ,TC.Username
     ,LC.Conversationid
     ,LC.SlidingExpirationDate
from TelegramChat TC
join (select
       Conversationid
        ,TelegramChatId
        ,SlidingExpirationDate
   from LLMConversation order by SlidingExpirationDate desc limit 1) LC
  on TC.ChatId = LC.TelegramChatId
