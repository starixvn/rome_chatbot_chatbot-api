**************************
Message Services
**************************

Get Personal Messages
--------------------
Describe the purpose of the service

  **Http Request:**

  ``GET: /api/{cid}/messages``

  **Query Paramaters:**

  ==========  =====  ============================
  Paramater   Type   Description
  ==========  =====  ============================
  cid         int    customer id
  ==========  =====  ============================

  **Request Headers:**

  ==========  ===============  ============================
  Paramater   Value            Description
  ==========  ===============  ============================
  :method     GET                 
  ==========  ===============  ============================


  **Response:**

  .. code-block:: json

    [
        { 
            "messageid": 1073779205, 
            "content" : "Testing message" 
        },
        { 
            "messageid": 1073779206, 
            "content" : "Testing message" 
        }
    ]