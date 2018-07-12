**************************
Introduction
**************************

**Chatbot services** is a single entry point for clients to access services across boundary. Currently, it includes three main components:

1.  **Notification service**: *a window service* periodically get data in order to send to Letstalk users.
2.  **Setting Services**: *a web service* provides function   
3.  **Auto Reply Service**: *a window service* to  


.. image:: assets/bot-services-intro.png

Chatbot API includes two main parts:

- Bot Client: a dll, every consumer site uses this client to access provided APIs
- Bot Services: 

    - receives requests related to bot, such as setting bot, send message to bot ...
    - run scheduled jobs to collect data and send to letstalk service
    - set auto reply messages in some cases like UM time

.. image:: assets/ChatBotAPI_Services.png  

In this implementation, we use *Message Pack* https://msgpack.org/index.html to serialize message content 