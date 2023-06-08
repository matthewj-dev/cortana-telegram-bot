# Cortana Telegram Bot Personal Project

## Original Objective

Creating a bot that would act as a representative between Microsoft's new BingChat-GPT4 and Telegram.

## Tech Stack

I decided to really Microsoft-ify this project.

* I named the bot Cortana.
* My GPT chat of choice was Bing.
* Runs on .NET.
* Using FSharp to make this a new experience, as I already have extensive CSharp experience.

## Previously used tech

This project was using SQLite to keep track of conversations with BingChat,
however after implementation it was discovered BingChat library has no way of letting the user handle conversations.
So I moved conversation handling to a singleton cache-esk type of thing...

Checkout commit 0bbffe83c7c7ebe4e1430b3976e45081853aaccb

## Concluding Thoughts

... (Will update when project reaches MVP)
