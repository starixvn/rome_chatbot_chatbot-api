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

  ``GET: /api/winloss/yesterday/cid``

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


  **Respone:**

  .. code-block:: json

    {
        "turnover": "30,000.00",
        "grosscomm": "50.00",
        "mastertotal": "50.00",
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

  ``GET: /api/winloss/today/cid``

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


  **Respone:**

  .. code-block:: json

    {
        "turnover": "30,000.00",
        "grosscomm": "50.00",
        "mastertotal": "50.00",
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

  ``GET: /api/oustanding/cid``

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


  **Respone:**

  .. code-block:: json

    {
        "totalmember": "1,000.00",
        "totalsuper": "50.00"
    }

--------------------------

Get Balance Info
----------------

**Http Request:**

  ``GET: /api/balance/cid``

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


  **Respone:**

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