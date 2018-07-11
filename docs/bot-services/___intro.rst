**************************
Introduction
**************************

ChatBot (Bot) API is a group of services, every request interacts with letstalk app, chatbot database ..., must go though these services.

.. image:: assets/bot-services-intro.png

Chatbot API includes two main parts:

- Bot Client: a dll, every consumer site uses this client to access provided APIs
- Bot Services: 

    - receives requests related to bot, such as setting bot, send message to bot ...
    - run scheduled jobs to collect data and send to letstalk service
    - set auto reply messages in some cases like UM time

.. image:: assets/ChatBotAPI_Services.png 

In this implementation, we use *Message Pack* https://msgpack.org/index.html to serialize message content