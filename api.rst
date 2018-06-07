API
===

set_key_to_contact
------------------

* Set encryption key to contact. (Level 2 only)
* Name: **​bot/set_key_to_contact/{{user_name}}**
* Method: POST
* Header::

	{
		Content-Type: application/json
		Authorization: Beare base64(api key)
	}
	
* Input: JSON object::

	{
		"contact": string,
		"encryption_key" : String
		"hash_id":long
	}
	
* Output: JSON object::

	{
		"status": int
		"message": string
	}

* Status:

+--------+--------------------+
|Code    | Description        |
+========+====================+
|0       | failure            |
+--------+--------------------+
|1       | success            |
+--------+--------------------+

set_webhook
-----------

* Setup you webhook URL
* Name: **bot/set_webhook/{{user_name}}**
* Method: POST
* Header::

	{
		Content-Type: application/json
		Authorization: Beare base64(api key)
	}
	
* Input: JSON object::

	{
		"url": string
	}
	
* Output: JSON object::

	{
		"status": int
		"message": string
	}

* Status:

+--------+--------------------+
|Code    | Description        |
+========+====================+
|0       | failure            |
+--------+--------------------+
|1       | success            |
+--------+--------------------+

get_profile
-----------

* Get contact profile information.
* Name: **​bot/get_profile/{{user_name}}**
* Method: GET
* Header::

	{
		Content-Type: application/json
		Authorization: Beare base64(api key)
	}
	
* Input: JSON object::

	{
		"contact": string
	}
	
* Output: JSON object::

	{
		"status": int
		"message": string,
		"avatar_url": string,
		"display_name": string
	}

* Status:

+--------+--------------------+
|Code    | Description        |
+========+====================+
|0       | failure            |
+--------+--------------------+
|1       | success            |
+--------+--------------------+

