***************
Report Services
***************

.. contents:: Table of Contents
   :local:
   :backlinks: top

Get Win-Loss
------------

Gets win loss report for customer by date.

  **Http Request:**

  ``GET: /api/report/{custId}/{custLevel}/winloss/{date}``

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
  custId      int    Customer id
  ----------  -----  -----------------------------------
  custLevel   int    Customer level
  ----------  -----  -----------------------------------
  date        long   Time of the report in milliseconds
  ==========  =====  ===================================

  **Response:**

  Returns the status code ``200`` and a resource in response body depending on level of customer.

  Response body sample in JSON format:

    - Customer level is Agent:

    .. code-block:: JSON

        {
            "Turnover": 30000,
            "GrossComm": 50,
            "MemberNetWinLoss.WinLoss": -50,
            "MemberNetWinLoss.Comm": -5,
            "MemberNetWinLoss.Total": -55,
            "AgentNetWinLoss.WinLoss": -50,
            "AgentNetWinLoss.Comm": 5,
            "AgentNetWinLoss.Total": -70,
            "companyWinLoss": -500
        }

    - Customer level is Master:

    .. code-block:: JSON

        {
            "Turnover": 30000,
            "GrossComm": 50,
            "AgentNetWinLoss.Total": -55,
            "MasterNetWinLoss.WinLoss": -50,
            "MasterNetWinLoss.Comm": 5,
            "MasterNetWinLoss.Total": -70,
            "companyWinLoss": -500
        }
    
    - Customer level is Super:

    .. code-block:: JSON

        {
            "Turnover": 30000,
            "GrossComm": 50,
            "MasterNetWinLoss.Total": -55,
            "SuperNetWinLoss.WinLoss": -50,
            "SuperNetWinLoss.Comm": 5,
            "SuperNetWinLoss.Total": -70,
            "companyWinLoss": -500
        }

----------------------

Get Outstanding
---------------

Gets oustanding report for customer.

  **Http Request:**

  ``GET: /api/report/{custId}/{custLevel}/outstanding``

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
  custId      int    Customer id
  ----------  -----  ----------------------------
  custLevel   int    Customer level
  ==========  =====  ============================

  **Response:**

  Returns the status code ``200`` and a resource in response body.

  Response body sample in JSON format:

    - Customer level is Agent:
    
    .. code-block:: json

        {
            "Member": 1000,
            "Agent": 50
        }
    
    - Customer level is Master:
    
    .. code-block:: json

        {
            "Member": 1000,
            "Master": 50
        }

    - Customer level is Super:
    
    .. code-block:: json

        {
            "Member": 1000,
            "Super": 50
        }


--------------------------

Get Balance Info
----------------

Gets balance info for customer

  **Http Request:**

  ``GET: /api/report/{custId}/balance``

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
  custId      int    customer id
  ==========  =====  ============================

  **Response:**

  Returns the status code ``200`` and a resource in response body.

  Response body sample in JSON format:

    - Customer level is Agent:

    .. code-block:: json

        {
            "CurrencyName": "UUS",
            "CashBalance": 1500000,
            "YesterdayCashBalance": 1500000,
            "TotalBalance": -120000,
            "YesterdayTotalBalance": -120906,
            "TodayWinLoss": 0,
            "YesterdayWinLoss": 0,
            "GivenCredit": 6000000,
            "MemberTotalCredit": 5630000
        }

    - Customer level is Master:

    .. code-block:: json

        {
            "CurrencyName": "UUS",
            "CashBalance": 1500000,
            "YesterdayCashBalance": 1500000,
            "TotalBalance": -120000,
            "YesterdayTotalBalance": -120906,
            "TodayWinLoss": 0,
            "YesterdayWinLoss": 0,
            "GivenCredit": 6000000,
            "AgentTotalCredit": 5630000
        }

    - Customer level is Super:

    .. code-block:: json

        {
            "CurrencyName": "UUS",
            "CashBalance": 1500000,
            "YesterdayCashBalance": 1500000,
            "TotalBalance": -120000,
            "YesterdayTotalBalance": -120906,
            "TodayWinLoss": 0,
            "YesterdayWinLoss": 0,
            "GivenCredit": 6000000,
            "MasterTotalCredit": 5630000
        }