*********************
Introduction
*********************

.. image:: assets/hld.png

With vertical slicing, project is to separate into three domains: **Alpha**, **Chatbot**, and **Letstalk**. And every part has a dedicated database of their own, that would enable `microservice architecture <https://martinfowler.com/articles/microservices.html>`_.

With horizontal slicing, from the traditional **n-architecture** point of view, the project is divided into three tiers.

1. **Presentation tier:** Letstalk app, Alpha setup page, and Chatbot backend site.
2. **Business tier:** A collection of scalable **microservices**, when each service perform only one operation read data (query) or update data (command). 
3. **Data tier:** Alpha service provides SMA's data such as *customer info*, `report`, `balance`, etc.



.. note::       

    - **CQRS** https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs

    - **Microservices** https://martinfowler.com/articles/microservices.html



