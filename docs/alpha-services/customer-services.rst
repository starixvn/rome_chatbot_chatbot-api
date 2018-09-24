*****************
Customer Services
*****************

.. contents:: Table of Contents
   :local:
   :backlinks: top

Is Account Closed
-----------------
Check status of account/ sub account.

  **Http Request:**

  ``GET: /api/customer/{custId}/isaccountclosed/{subAccountId}``

  **Request Headers:**

  ===============  ============================
  Request Header   Description
  ===============  ============================
   Authorization    Bearer ``<token>``             
  ===============  ============================

  **Query Paramaters:**

  =============  =====  ============================
  Paramater      Type   Description
  =============  =====  ============================
  custId         int    Customer id
  -------------  -----  ----------------------------
  subAccuntId    int    Sub account id
  =============  =====  ============================

  **Response:**

  Returns the status code ``200`` and a resource in response body.

  Response body sample in JSON format:

  .. code-block:: json

    False