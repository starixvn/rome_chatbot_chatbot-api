****************
Message Services
****************

.. contents:: Table of Contents
   :local:
   :backlinks: top

Get Messages
------------
Gets personal messages, normal/special announcements for customers.

  **Http Request:**

  ``POST: /api/message``

  **Request Headers:**

  ===============  ============================
  Request Header   Description
  ===============  ============================
   Authorization    Bearer ``<token>``             
  ===============  ============================

  **Request Body:**

  ============  ==========================  ===========================================================================
  Paramater     Type                        Description
  ============  ==========================  ===========================================================================
  messageType   int                         Message Type (0: normal message, 1: personal message, 2: special message )
  ------------  --------------------------  ---------------------------------------------------------------------------
  fromTime      long                        Start time to query messages in milliseconds.
  ------------  --------------------------  ---------------------------------------------------------------------------
  toTime        long                        End time to query messages in milliseconds.
  ------------  --------------------------  ---------------------------------------------------------------------------
  customers     Array of customer object    List of customer need to query messages
  ============  ==========================  ===========================================================================

  Request body sample in JSON format:

  .. code-block:: json

    {
        "messageType": 1,
        "fromTime": 1537770486857,
        "toTime": 1537770500450,
        "customers": [
            {
            "custId": 1,
            "roleId": 2,
            "language": "en-US"
            }
        ]
    }

  **Response:**

  Returns the status code ``200`` and an array of message.

  Response body sample in JSON format:

  .. code-block:: json

    [
        { 
            "custid": 1, 
            "content" : "Testing message" 
        },
        { 
            "custid": 1, 
            "content" : "Testing message" 
        }
    ]