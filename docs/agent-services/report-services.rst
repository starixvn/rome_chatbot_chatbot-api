***************
Report Services
***************

.. contents:: Table of Contents
   :local:
   :backlinks: top

Get Yesterday Win-Loss
----------------------
Describe the purpose of the service


  **Http Request:**

  ``GET: /api/winloss/yesterday/{cid}``

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

  * Level Agent:

  .. code-block:: json

    {
        "turnover": "30,000.00",
        "grosscomm": "50.00",
        "member": {
            "winloss": "-50.00",
            "comm": "-5.00",
            "total": "-55.00"
        },
        "agent": {
            "winloss": "-50.00",
            "comm": "-5.00",
            "total": "-70.00"
        },
        "master": null,
        "super": null,
        "company": "-500.00"
    }

  * Level Master:

  .. code-block:: json

    {
        "turnover": "30,000.00",
        "grosscomm": "50.00",
        "member": null,
        "agent": {
            "winloss": null,
            "comm": null,
            "total": "-70.00"
        },
        "master": {
            "winloss": "-50.00",
            "comm": "-5.00",
            "total": "-70.00"
        },
        "super": null,
        "company": "-500.00"
    }

  * Level Super:

  .. code-block:: json

    {
        "turnover": "30,000.00",
        "grosscomm": "50.00",
        "member": null,
        "agent": null,
        "master": {
            "winloss": null,
            "comm": null,
            "total": "-70.00"
        },
        "super": {
            "winloss": "-50.00",
            "comm": "-5.00",
            "total": "-70.00"
        },
        "company": "-500.00"
    }

----------------------

Get Today Win-Loss
------------------

Describe the purpose of the service

  **Http Request:**

  ``GET: /api/winloss/today/{cid}``

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

  * Level Agent:

  .. code-block:: json

    {
        "turnover": "30,000.00",
        "grosscomm": "50.00",
        "member": {
            "winloss": "-50.00",
            "comm": "-5.00",
            "total": "-55.00"
        },
        "agent": {
            "winloss": "-50.00",
            "comm": "-5.00",
            "total": "-70.00"
        },
        "master": null,
        "super": null,
        "company": "-500.00"
    }

  * Level Master:

  .. code-block:: json

    {
        "turnover": "30,000.00",
        "grosscomm": "50.00",
        "member": null,
        "agent": {
            "winloss": null,
            "comm": null,
            "total": "-70.00"
        },
        "master": {
            "winloss": "-50.00",
            "comm": "-5.00",
            "total": "-70.00"
        },
        "super": null,
        "company": "-500.00"
    }

  * Level Super:

  .. code-block:: json

    {
        "turnover": "30,000.00",
        "grosscomm": "50.00",
        "member": null,
        "agent": null,
        "master": {
            "winloss": null,
            "comm": null,
            "total": "-70.00"
        },
        "super": {
            "winloss": "-50.00",
            "comm": "-5.00",
            "total": "-70.00"
        },
        "company": "-500.00"
    }

--------------------------

Get Outstanding
---------------

  **Http Request:**

  ``GET: /api/oustanding/{cid}``

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

    {
        "outstanding": "1,000.00",
        "totaloutstanding": "50.00"
    }

--------------------------

Get Balance Info
----------------

**Http Request:**

  ``GET: /api/balance/{cid}``

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

    {
        "currency": "UUS",
        "cashbalance": "1,500,000.00",
        "yesterdaycashbalance": "1,500,000.00",
        "totalbalance": "-120,000.00",
        "yesterdaytotalbalance": "-120,906.77",
        "todaywinloss": "0.00",
        "yesterdaywinloss": "0.00",
        "givencredit": "6,000,000.00",
        "membercredit": "5,630,000.00"
    }