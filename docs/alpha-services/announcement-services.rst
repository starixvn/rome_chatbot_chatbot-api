*********************
Announcement Services
*********************

.. contents:: Table of Contents
   :local:
   :backlinks: top

Get Announcements
------------
Gets system announcements for customers.

  **Http Request:**

  ``POST: /announcements``

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
  fromTime      long                        Start time to query announcements in milliseconds.
  ------------  --------------------------  ---------------------------------------------------------------------------
  toTime        long                        End time to query announcements in milliseconds.
  ------------  --------------------------  ---------------------------------------------------------------------------
  customers     Array of customer object    List of customer need to query announcements
  ============  ==========================  ===========================================================================

  Request body sample in JSON format:

  .. code-block:: json

    {
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