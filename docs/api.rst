API
===

change_display_name
-------------------

To change the display name of business accounts.

Name: **bot/change_display_name/{{user_name}}**

Method: ``PUT``

* Header::

	{
		Content-Type : application/json
		Authorization : Bearer base64(api key)
	}
	
*  Input: JSON object::

	{
		"display_name": String
	}
	
*  Output: JSON object::

	{
		"status": int
		"message": String
	}
	
Status:

+------------+-----------------------------------------------------------------+
| Code       | Description                                                     |
+============+=================================================================+
|0           | failure                                                         |
+------------+-----------------------------------------------------------------+
|1           | success                                                         |
+------------+-----------------------------------------------------------------+

upload_avatar_image
-------------------

* To upload avatar/portrait images in 640x640.
* Name: **​bot/upload_avatar_image/{{user_name}}**
* Method: POST
* Header::
	
	{
		Content-Type : application/json
		Authorization : Bearer base64(api key)
	}
	
*  Input: JSON object::

	{
		"image": String
	}
	
*  Output: JSON object::

	{
		"status": int
		"message": String
	}
	
* Status:

	+------------+-----------------------------------------------------------------+
	| Code       | Description                                                     |
	+============+=================================================================+
	|0           | failure                                                         |
	+------------+-----------------------------------------------------------------+
	|1           | success                                                         |
	+------------+-----------------------------------------------------------------+

set_welcome_msg
---------------

* To setup welcome messages for new contact.
* Name: **​ bot/set_welcome_msg/{{user_name}}**
* Method: POST
* Header::
	
	{
		Content-Type : application/json
		Authorization : Bearer base64(api key)
	}
	
*  Input: JSON object::

	{
		"message": String
	}
	
*  Output: JSON object::

	{
		"status": int
		"message": String
	}
	
* Status:

	+------------+-----------------------------------------------------------------+
	| Code       | Description                                                     |
	+============+=================================================================+
	|0           | failure                                                         |
	+------------+-----------------------------------------------------------------+
	|1           | success                                                         |
	+------------+-----------------------------------------------------------------+

set_auto_reply
--------------

* To set a message auto replied to contact’s messages.
* Name: **​bot/set_auto_reply/{{user_name}}**
* Method: POST
* Header::
	
	{
		Content-Type : application/json
		Authorization : Bearer base64(api key)
	}
	
*  Input: JSON object::

	{
		"message": String,
		"enable": int(0:off 1:on)
	}
	
*  Output: JSON object::

	{
		"status": int
		"message": String
	}
	
* Status:

	+------------+-----------------------------------------------------------------+
	| Code       | Description                                                     |
	+============+=================================================================+
	|0           | failure                                                         |
	+------------+-----------------------------------------------------------------+
	|1           | success                                                         |
	+------------+-----------------------------------------------------------------+

push
----

* To send a message to contact.
* Name: **bot/push/{{user_name}}**
* Method: POST
* Header::
	
	{
		Content-Type : application/json
		Authorization : Bearer base64(api key)
	}
	
*  Input: JSON object::

	{
		"receiver": Array [receiver A, receiver B...] MAX:50
		"payload": String
	}
	
*  Output: JSON object::

	{
		"status": int
		"message": String
	}
	
* Status:

	+------------+-----------------------------------------------------------------+
	| Code       | Description                                                     |
	+============+=================================================================+
	|0           | failure                                                         |
	+------------+-----------------------------------------------------------------+
	|1           | success                                                         |
	+------------+-----------------------------------------------------------------+

push_to_tag
-----------

* To send message to tagged contacts.
* Name: **bot/push_to_tag/{{user_name}}**
* Method: POST
* Header::
	
	{
		Content-Type : application/json
		Authorization : Bearer base64(api key)
	}
	
*  Input: JSON object::

	{
		"tag": String
		"payload": String
	}
	
*  Output: JSON object::

	{
		"status": int
		"message": String
	}
	
* Status:

	+------------+-----------------------------------------------------------------+
	| Code       | Description                                                     |
	+============+=================================================================+
	|0           | failure                                                         |
	+------------+-----------------------------------------------------------------+
	|1           | success                                                         |
	+------------+-----------------------------------------------------------------+

modify_tag
----------

* To add or delete tags, number of tags are limited to 20.
* Name: **bot/modify_tag/{{user_name}}**
* Method: POST/DELETE
* Header::
	
	{
		Content-Type : application/json
		Authorization : Bearer base64(api key)
	}
	
*  Input: JSON object::

	{
		"tag": String
	}
	
*  Output: JSON object::

	{
		"status": int
		"message": String
	}
	
* Status:

	+------------+-----------------------------------------------------------------+
	| Code       | Description                                                     |
	+============+=================================================================+
	|0           | failure                                                         |
	+------------+-----------------------------------------------------------------+
	|1           | success                                                         |
	+------------+-----------------------------------------------------------------+

list_tag
--------

* To list all tags
* Name: **bot/list_tag/{{user_name}}**
* Method: GET
* Header::
	
	{
		Content-Type : application/json
		Authorization : Bearer base64(api key)
	}
	
*  Input: None
	
*  Output: JSON object::

	{
		"status": int
		"message": String
		"tags": [String, ...]
	}
	
* Status:

	+------------+-----------------------------------------------------------------+
	| Code       | Description                                                     |
	+============+=================================================================+
	|0           | failure                                                         |
	+------------+-----------------------------------------------------------------+
	|1           | success                                                         |
	+------------+-----------------------------------------------------------------+

get_contacts
------------

* To get contacts by tags, list all contacts when tag is not provided
* Name: **​bot/get_contacts/{{user_name}}?tag=xxx**
* Method: GET
* Header::
	
	{
		Content-Type : application/json
		Authorization : Bearer base64(api key)
	}
	
*  Input: None
	
*  Output: JSON object::

	{
		"status": int
		"message": String,
		"tag":(String),
		"contacts": [
			{"user_id":String (Letstalk id),"avatar_url":String, "display_name":String},
			{"user_id":String (Letstalk id),"avatar_url":String, "display_name":String},
			...
		]
	}
	
* Status:

	+------------+-----------------------------------------------------------------+
	| Code       | Description                                                     |
	+============+=================================================================+
	|0           | failure                                                         |
	+------------+-----------------------------------------------------------------+
	|1           | success                                                         |
	+------------+-----------------------------------------------------------------+

modify_contact_tag
------------------

* To tag or to remove the tag of some contact.
* Name: **​​bot/modify_contact_tag/{{user_name}}**
* Method: POST/DELETE
* Header::
	
	{
		Content-Type : application/json
		Authorization : Bearer base64(api key)
	}
	
*  Input: JSON object::
	
	{
		"contact":String,
		"tag":String
	}
	
*  Output: JSON object::

	{
		"status": int
		"message": String
	}
	
* Status:

	+------------+-----------------------------------------------------------------+
	| Code       | Description                                                     |
	+============+=================================================================+
	|0           | failure                                                         |
	+------------+-----------------------------------------------------------------+
	|1           | success                                                         |
	+------------+-----------------------------------------------------------------+

set_hash_key
------------

* Upload new AES key, if encryption level 2 does not allow uploaded
* Name: **​​bot/set_hash_key/{{user_name}}**
* Method: POST
* Header::
	
	{
		Content-Type : application/json
		Authorization : Bearer base64(api key)
	}
	
*  Input: JSON object::
	
	{
		"hash_key": String
		"hash_id":longlong
	}
	
*  Output: JSON object::

	{
		"status": int
		"message": String
	}
	
* Status:

	+------------+-----------------------------------------------------------------+
	| Code       | Description                                                     |
	+============+=================================================================+
	|0           | failure                                                         |
	+------------+-----------------------------------------------------------------+
	|1           | success                                                         |
	+------------+-----------------------------------------------------------------+

.. _get-contact-public-key:
	
get_contact_public_key
----------------------

* Get contact RSA public key. (Level 2 only)
* Name: **​bot/get_contact_public_key/{{user_name}}?contact=xxxx**
* Method: GET
* Header::
	
	{
		Content-Type : application/json
		Authorization : Bearer base64(api key)
	}
	
*  Input: None
	
*  Output: JSON object::

	{
		"status": int
		"message": String
		"public_key": String
	}
	
* Status:

	+------------+-----------------------------------------------------------------+
	| Code       | Description                                                     |
	+============+=================================================================+
	|0           | failure                                                         |
	+------------+-----------------------------------------------------------------+
	|1           | success                                                         |
	+------------+-----------------------------------------------------------------+
	
.. _set-key-to-contact:
	
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

	+------------+-----------------------------------------------------------------+
	| Code       | Description                                                     |
	+============+=================================================================+
	|0           | failure                                                         |
	+------------+-----------------------------------------------------------------+
	|1           | success                                                         |
	+------------+-----------------------------------------------------------------+
	
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

	+------------+-----------------------------------------------------------------+
	| Code       | Description                                                     |
	+============+=================================================================+
	|0           | failure                                                         |
	+------------+-----------------------------------------------------------------+
	|1           | success                                                         |
	+------------+-----------------------------------------------------------------+
	
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

	+------------+-----------------------------------------------------------------+
	| Code       | Description                                                     |
	+============+=================================================================+
	|0           | failure                                                         |
	+------------+-----------------------------------------------------------------+
	|1           | success                                                         |
	+------------+-----------------------------------------------------------------+