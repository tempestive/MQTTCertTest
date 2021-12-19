# MQTTCertTest

A very simple program, written on .NET 6 using Visual Studio 2022, to check the expiration date of a certificate of a MQTT broker.


It uses the MQTTnet library https://github.com/chkr1011/MQTTnet.


Usage:

`
MQTTCertTest host user password expiration-date
`

expiration-date is in a format that can be parsed by DateTime.Parse method. For example 2022-03-01.
The program show the result to the stdout and have an exit code of 0 if the current certificate has an expiration 
date that is older than expiration-date, <>0 otherwise.

Note that the code is very raw.


