****************
Message Services
****************

.. contents:: Table of Contents
   :local:
   :backlinks: top

Get Messages
------------
Gets personal messages, normal/special announcements for customer.

  **Http Request:**

  ``GET: /api/message/{custId}/messages/{messageType}/{language}/{fromTime}/{toTime}``

  **Request Headers:**

  ===============  ============================
  Request Header   Description
  ===============  ============================
   Authorization    Bearer ``<token>``             
  ===============  ============================

  **Query Paramaters:**

  ============  =======  ===========================================================================
  Paramater     Type     Description
  ============  =======  ===========================================================================
  custId        int      customer id
  ------------  -------  ---------------------------------------------------------------------------
  messageType   int      Message Type (0: normal message, 1: personal message, 2: special message )
  ------------  -------  ---------------------------------------------------------------------------
  language      string   language code (en-US, zh-TW, zh-CN, ja-JP, th-TH, ko-KR, vi-VN)
  ------------  -------  ---------------------------------------------------------------------------
  fromTime      long     Start time to query messages in milliseconds.
  ------------  -------  ---------------------------------------------------------------------------
  toTime        long     End time to query messages in milliseconds.
  ============  =======  ===========================================================================

  **Response:**

  Returns the status code ``200`` and an array of message.

  Response body sample in JSON format:

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