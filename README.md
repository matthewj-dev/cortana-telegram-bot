# Cortana Telegram Bot Personal Project

## Original Objective

1. Learning FSharp
2. Creating a bot that would act as a representative between Microsoft's new BingChat-GPT4 and Telegram.

## Implementation Details

I decided to really Microsoft-ify this project.

* I named the bot Cortana, after the character from Halo.
* My GPT chat of choice was Bing. Using this reverse engineered library. [https://github.com/topics/bing-chat](https://github.com/topics/bing-chat)
* Runs on .NET.
* Using FSharp to make this a new experience, as I already have extensive CSharp experience.
* Using this Telegram library to interact with Telegram. [https://github.com/Dolfik1/Funogram](https://github.com/Dolfik1/Funogram)

## Previously used tech

* A brief, but very rocky foray into using LiteDb with FSharp. Don't even try it.

* This project was using SQLite to keep track of conversations with BingChat, however after implementation it was discovered BingChat library has no way of letting the user handle conversations. So I moved conversation handling to a singleton cache-esk type of thing...

Checkout commit 0bbffe83c7c7ebe4e1430b3976e45081853aaccb

## Concluding Thoughts

Over the course of this project, I've grown to like FSharp's interesting syntax. Reading FSharp, given some adjustment type, can be pleasant and terse. A log of this can be traced to the strong types with powerful type inferring. There are some minor things I was not so thrilled about, such as the linter always telling me I should use `|> ignore` when the “return” is just `unit`. The linter itself is also somewhat primitive and gave confusing or misleading error descriptions. Having instant access to the entire NuGet library of packages is also a massive boon, and when whatever library you want doesn't operate idiomatically, there is usually some sort of bridging library available. That being said, the latter can sometimes come with tradeoffs or out-of-date bindings. If being considered for use in production code, I would recommend: a) committing to the language as a team and b) heavily investigating any libraries or frameworks that a project might need.

Personally, I will most likely not use FSharp for future personal projects. The library situation was frustrating enough to make some parts of this process not fun. The syntax, however, is very intriguing, and I intend on writing something in OCaml in the future.
