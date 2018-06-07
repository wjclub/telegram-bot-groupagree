#!/bin/bash
while [ ! -f cancer.wjdummy ]
do
	mono telegram-bot-groupagree.exe <dbuser> <dbpw> <dbname>
done
