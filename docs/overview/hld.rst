==================
High Level Design
==================

.. image:: assets/hld.png

With vertical slicing, the project is to separate into three domains: **Alpha**, **Chatbot**, and **Letstalk**. Every part has a dedicated database of their own, that would enable **microservice architecture**. [#microservice]_

.. note::       
    -  Alpha team takes responsibilities on Alpha and Chatbot. 

    -  Letstalk team is responsible for all things belong to Letstalk platform.

With horizontal slicing, from the traditional **n-architecture** point of view, the project is divided into three tiers:

1. **Presentation tier:** Letstalk app, Alpha chatbot setup page, and Chatbot backend site.
2. **Business tier:** A collection of scalable **microservices**, when each service perform only one operation read data (query) or update data (command), CQRS [#cqrs]_ pattern may be applied.
3. **Data tier:** Alpha service provides SMA's data such as *customer info*, `report`, `balance`, etc.



References       
==========
.. [#cqrs] CQRS https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs

.. [#microservice] Microservice https://martinfowler.com/articles/microservices.html



