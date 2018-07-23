***************
Report Services
***************

.. contents:: Table of Contents
   :local:
   :backlinks: top

Get Win-Loss
----------------------

Gets win loss report for customer by date.

  **Http Request:**

  ``GET: /api/report/{custId}/winloss/{date}``

  **Request Headers:**

  ===============  ============================
  Request Header   Description
  ===============  ============================
  Authorization    Bearer ``<token>``          
  ===============  ============================

  **Query Paramaters:**

  ==========  =====  ===================================
  Paramater   Type   Description
  ==========  =====  ===================================
  cid         int    customer id
  ----------  -----  -----------------------------------
  date        long   Time of the report in milliseconds
  ==========  =====  ===================================

  **Response:**

  Returns the status code ``200`` and a resource in response body depending on level of customer.

  Response body sample in JSON format:

    .. hlist::
        :columns: 3

        - Member Yesterday Win Loss (Customer level is Agent)

            .. code-block:: JSON

                {
                    "turnover": 30000,
                    "grosscomm": 50,
                    "memberwinloss": {
                        "winloss": -50,
                        "comm": -5,
                        "total": -55
                    },
                    "agentwinloss": {
                        "winloss": -50,
                        "comm": 5,
                        "total": -70
                    },
                    "masterwinloss": null,
                    "superwinloss": null,
                    "company": -500
                }

        - Agent Yesterday Win Loss (Customer level is Master)

            .. code-block:: JSON

                {
                    "turnover": 30000,
                    "grosscomm": 50,
                    "memberwinloss": null,
                    "agentwinloss": {
                        "winloss": null,
                        "comm": null,
                        "total": -70
                    },
                    "masterwinloss": {
                        "winloss": -50,
                        "comm": -5,
                        "total": -70
                    },
                    "superwinloss": null,
                    "company": -500
                }

        - Master Yesterday Win Loss (Customer level is Super)

            .. code-block:: JSON

                {
                    "turnover": 30000,
                    "grosscomm": 50,
                    "memberwinloss": null,
                    "agentwinloss": null,
                    "masterwinloss": {
                        "winloss": null,
                        "comm": null,
                        "total": -70
                    },
                    "superwinloss": {
                        "winloss": -50,
                        "comm": -5,
                        "total": -70
                    },
                    "company": -500
                }

----------------------

Get Outstanding
---------------

Gets oustanding report for customer.

  **Http Request:**

  ``GET: /api/report/oustanding/{cid}``

  **Request Headers:**

  ===============  ============================
  Request Header   Description
  ===============  ============================
  Authorization    Bearer ``<token>``          
  ===============  ============================

  **Query Paramaters:**

  ==========  =====  ============================
  Paramater   Type   Description
  ==========  =====  ============================
  cid         int    customer id
  ==========  =====  ============================

  **Response:**

  Returns the status code ``200`` and a resource in response body.

  Response body sample in JSON format:

  .. code-block:: json

    {
        "levelid": 2,
        "totaloutstanding": 1000,
        "outstanding": 50
    }

--------------------------

Get Balance Info
----------------

Gets balance info for customer

  **Http Request:**

  ``GET: /api/report/balance/{cid}``

  **Request Headers:**

  ===============  ============================
  Request Header   Description
  ===============  ============================
  Authorization    Bearer ``<token>``          
  ===============  ============================

  **Query Paramaters:**

  ==========  =====  ============================
  Paramater   Type   Description
  ==========  =====  ============================
  cid         int    customer id
  ==========  =====  ============================

  **Response:**

  Returns the status code ``200`` and a resource in response body.

  Response body sample in JSON format:

  .. code-block:: json

    {
        "currency": "UUS",
        "cashbalance": 1500000,
        "yesterdaycashbalance": 1500000,
        "totalbalance": -120000,
        "yesterdaytotalbalance": -120906,
        "todaywinloss": 0,
        "yesterdaywinloss": 0,
        "givencredit": 6000000,
        "membercredit": 5630000
    }