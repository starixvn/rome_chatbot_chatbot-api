******
Errors
******

.. contents:: Table of Contents
   :local:
   :backlinks: top

Status Codes
------------

The following status codes are returned by the Alpha Services.

    ============  =======================  =======================================
    Status Code   Title                    Description
    ============  =======================  =======================================
    200           OK                       The request was successful.
    ------------  -----------------------  ---------------------------------------
    400           Bad request              Bad request                           
    ------------  -----------------------  ---------------------------------------
    401           Unauthorized             API key is invalid.
    ------------  -----------------------  ---------------------------------------
    403           Forbidden                Not authorized to access the resource.
    ------------  -----------------------  ---------------------------------------
    404           Not found                The resource does not exist.
    ------------  -----------------------  ---------------------------------------
    500           Internal Server Error    Error on the internal server
    ============  =======================  =======================================

Error Types
-----------

    =====================  ============================================================
    Type                   Description
    =====================  ============================================================
    authentication_error   Failure to properly authenication yourself in the request.
    ---------------------  ------------------------------------------------------------
    params_invalid         Your parameters were not valid.
    ---------------------  ------------------------------------------------------------
    unknow_route           URL was not valid
    ---------------------  ------------------------------------------------------------
    api_error              Internal API error.
    =====================  ============================================================  

Error Responses
---------------

The following table describes the elemements that can appear int the response body of an error.

    ==========  ========  ============================================================
    Property    Type      Description
    ==========  ========  ============================================================
    type        string       An HTTP status code value, without the textual description.
    ----------  --------  ------------------------------------------------------------
    message     String    Description of the error.
    ==========  ========  ============================================================

    Error response sample in JSON format:

    .. code-block:: json

        {
            "error": {
                "type": "authentication_error",
                "message": "You are not authorized on this function."
            }
        }