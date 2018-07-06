***************
Report Services
***************

.. contents:: Table of Contents

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

--------------------------

Get Today Win-Loss
----------------------


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



Get Outstanding
----------------------
Etiam turpis ante, luctus sed velit tristique, finibus volutpat dui. Nam sagittis vel ante nec malesuada.
Praesent dignissim mi nec ornare elementum. Nunc eu augue vel sem dignissim cursus sed et nulla.


Get Balance Info
----------------------
Etiam turpis ante, luctus sed velit tristique, finibus volutpat dui. Nam sagittis vel ante nec malesuada.
Praesent dignissim mi nec ornare elementum. Nunc eu augue vel sem dignissim cursus sed et nulla.
