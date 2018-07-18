**************************
Introduction
**************************

Every message sent to **Chatbot** will trigger **Letstalk platform (sender)** to post a request to **Chatbot webhook (receiver)**.

Firstly, webhook authenticates the request to make sure it sent by **Letstalk service**, and then do some necessary actions before forwarding the request to **Chatbot services** for further business processing.

.. image:: assets/HL_Webhook.png


**************************
Command
**************************

Command is text that SMA chat with bot, command can be ``outstanding``, ``win loss`` or ``balance``. 
This sequence diagram below describes how message pass to webhook and return result message to user.


.. image:: assets/SQ_Webhook_Command.png
 
