***************
Report Services
***************

.. contents:: Table of Contents
   :local:
   :backlinks: top

Get Yesterday Win-Loss
----------------------

Gets yesterday win loss report for customer.


  **Http Request:**

  ``GET: /api/winloss/yesterday/{cid}``

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

  Returns the status code ``200`` and a JSON object depending on level of customer.

    .. hlist::
        :columns: 3

        - Member Yesterday Win Loss (Customer level is Agent)

            .. code-block:: JSON

                {
                    "turnover": 30000,
                    "grosscomm": 50,
                    "member": {
                        "winloss": -50,
                        "comm": -5,
                        "total": -55
                    },
                    "agent": {
                        "winloss": -50,
                        "comm": 5,
                        "total": -70
                    },
                    "master": null,
                    "super": null,
                    "company": -500
                }


        - Agent Yesterday Win Loss (Customer level is Master)

            .. code-block:: JSON

                {
                    "turnover": 30000,
                    "grosscomm": 50,
                    "member": null,
                    "agent": {
                        "winloss": null,
                        "comm": null,
                        "total": -70
                    },
                    "master": {
                        "winloss": -50,
                        "comm": -5,
                        "total": -70
                    },
                    "super": null,
                    "company": -500
                }

        - Master Yesterday Win Loss (Customer level is Super)

            .. code-block:: JSON

                {
                    "turnover": 30000,
                    "grosscomm": 50,
                    "member": null,
                    "agent": null,
                    "master": {
                        "winloss": null,
                        "comm": null,
                        "total": -70
                    },
                    "super": {
                        "winloss": -50,
                        "comm": -5,
                        "total": -70
                    },
                    "company": -500
                }

----------------------

Get Today Win-Loss
------------------

Gets today win loss report for customer.

  **Http Request:**

  ``GET: /api/winloss/today/{cid}``

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

  Returns the status code ``200`` and a JSON object depending on level of customer.

    .. hlist::
        :columns: 3

        - Member Today Win Loss (Customer level is Agent)

            .. code-block:: JSON

                {
                    "turnover": 30000,
                    "grosscomm": 50,
                    "member": {
                        "winloss": -50,
                        "comm": -5,
                        "total": -55
                    },
                    "agent": {
                        "winloss": -50,
                        "comm": 5,
                        "total": -70
                    },
                    "master": null,
                    "super": null,
                    "company": -500
                }


        - Agent Today Win Loss (Customer level is Master)

            .. code-block:: JSON

                {
                    "turnover": 30000,
                    "grosscomm": 50,
                    "member": null,
                    "agent": {
                        "winloss": null,
                        "comm": null,
                        "total": -70
                    },
                    "master": {
                        "winloss": -50,
                        "comm": -5,
                        "total": -70
                    },
                    "super": null,
                    "company": -500
                }

        - Master Today Win Loss (Customer level is Super)

            .. code-block:: JSON

                {
                    "turnover": 30000,
                    "grosscomm": 50,
                    "member": null,
                    "agent": null,
                    "master": {
                        "winloss": null,
                        "comm": null,
                        "total": -70
                    },
                    "super": {
                        "winloss": -50,
                        "comm": -5,
                        "total": -70
                    },
                    "company": -500
                }

--------------------------

Get Outstanding
---------------

Gets oustanding report for customer.

  **Http Request:**

  ``GET: /api/oustanding/{cid}``

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

  Returns the status code ``200`` and a JSON object.

  .. code-block:: json

    {
        "totaloutstanding": 1000,
        "outstanding": 50
    }

--------------------------

Get Balance Info
----------------

Gets balance info for customer

  **Http Request:**

  ``GET: /api/balance/{cid}``

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

  Returns the status code ``200`` and a JSON object.

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