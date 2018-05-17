#!/bin/bash
while [ ! -f cancer.wjdummy ]
do
	mono telegram-bot-groupagree.exe gabetauser gabetapw groupagreebot_beta
done
