Introduction
============

This document covered APIs, webhook events and supported message types of Letstalk Business Accounts. There are two security levels, one can choose to integrate either ways to meet security requirements.

* **Level 1**: Letstalk service will do the RSA for end-to-end encryption, the AES key will be stored in the Letstalk service, provides easy integration with basic security.


* **Level 2**: Customers' bots take the responsibility to do end-to-end encryption by RSA, the Letstalk service will not be aware of any AES keys, this level provides advanced security.

Customers’ bots should use AES to encrypt all the message content in both levels.

We will provide sandbox environment for integration test.

The necessary parameters for applying for an account include ``Account Name``, ``Password``, ``Display Name``, ``Security level``. 

We will give you ``account_id`` **(Number)**, ``api_key``, ``hash_key`` and ``hash_id``.
