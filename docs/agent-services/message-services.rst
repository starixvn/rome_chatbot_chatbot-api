**************************
Message Services
**************************

.. contents:: Table of Contents
   :local:
   :backlinks: top

Get Messages
--------------------
Gets personal/special/normal messages for customer.

  **Http Request:**

  ``GET: /api/{cid}/messages/{messageType}``

  **Request Headers:**

  ===============  ============================
  Request Header   Description
  ===============  ============================
   Authorization    Bearer ``<token>``             
  ===============  ============================

  **Query Paramaters:**

  ============  =====  ===========================================================================
  Paramater     Type   Description
  ============  =====  ===========================================================================
  cid           int    customer id
  ------------  -----  ---------------------------------------------------------------------------
  messageType   int    Message Type (0: normal message, 1: personal message, 2: special message )
  ============  =====  ===========================================================================

  **Response:**

  Returns the status code ``200`` and a JSON object.

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

----------------------

Update Status Of Messages
-------------------------
Describe the purpose of the service

  **Http Request:**

  ``POST: /api/messages/updatestatus``
  
  **Request Headers:**

  ===============  ============================
  Request Header   Description
  ===============  ============================
  Content-Type     application/json            
  ---------------  ----------------------------
  Authorization    Bearer ``<token>``            
  ===============  ============================

  **Request Body:**

  ===========  =============  ============================
  Paramater    Type           Description
  ===========  =============  ============================
  messageids   Array of int   Array of messages IDs.
  ===========  =============  ============================

  **Response:**

  Return the status code ``200`` and an empty JSON object.