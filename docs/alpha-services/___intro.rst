************
Introduction
************

.. contents:: Table of Contents
   :local:
   :backlinks: top

Overview
--------

.. image:: assets/alpha-service-intro.png    
    :align: left

Common Specifications
---------------------

Status Codes
~~~~~~~~~~~~

The following status codes are returned by the Agent Services.

    ============  =======================================
    Status Code   Description
    ============  =======================================
    200           Request successful
    ------------  ---------------------------------------
    400           Bad Request
    ------------  ---------------------------------------
    401           Unauthorized
    ------------  ---------------------------------------
    403           Forbidden
    ------------  ---------------------------------------
    429           Too Many Requests
    ------------  ---------------------------------------
    500           Internal Server Error
    ============  =======================================

Error Responses
~~~~~~~~~~~~~~~

The folowing JSON data is returned in the response body when an error occurs.

    ==========  ========  ================================================
    Property    Type      Description
    ==========  ========  ================================================
    message     String    Message containing information about the error.
    ==========  ========  ================================================

    Example error response:

    .. code-block:: json

        {
            "message": "You are not authorized on this function."
        }