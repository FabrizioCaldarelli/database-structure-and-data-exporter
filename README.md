Database Structure And Data Exporter
======================================

This software is made to simplfy structure and data exports from a database source.

At this moment, the only supported platform is SQL Server (C# has been used to better interface to SQL Server).

How to use
==========

Compile and Launch.

Then you have to compile few fields:

1. Host: Server hostname. Now the code supports only trused connection, so it does not need username/password;
2. Database: Database name;
3. Namespace sources: Namespace to use when you will export model classes for C#, Java, ...;

Finally, you have to click on "PARSE STRUCTURE AND DATA" to collect data from database tables.

Next fields immediately after this button allows to specify final path where store exported json. Then click on "EXPORT TABLE DATA TO JSON" to complete the operation.

Again, last two controls refers to final path where store exported C# models. Click on "EXPORT C# MODELS" to complete the operation.
