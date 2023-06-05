create table TelegramChat
(
    ChatId   integer not null
        constraint TelegramChat_pk
            primary key autoincrement ,
    Type     TEXT    not null,
    Name     TEXT    not null,
    Username TEXT
);
