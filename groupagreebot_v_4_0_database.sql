CREATE DATABASE  IF NOT EXISTS /*your bots chat id here*/`insert_bot_chat_id` !40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE /*your bots chat id here*/`insert_bot_chat_id`;

DROP TABLE IF EXISTS `pointer`;

CREATE TABLE `pointer` (
  `chatId` int(32) NOT NULL,
  `needle` int(8) DEFAULT NULL,
  `anony` bit(1) DEFAULT NULL,
  `pollType` int(8) DEFAULT NULL,
  `boardChatId` int(32) DEFAULT NULL,
  `boardPollId` int(32) DEFAULT NULL,
  `lastPollId` int(32) NOT NULL DEFAULT '0',
  `lang` int(8) NOT NULL DEFAULT '0',
  PRIMARY KEY (`chatId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

DROP TABLE IF EXISTS `polls`;

CREATE TABLE `polls` (
  `chatid` int(32) NOT NULL,
  `pollid` int(32) NOT NULL,
  `pollText` mediumtext COLLATE utf8mb4_unicode_ci NOT NULL,
  `pollDescription` mediumtext COLLATE utf8mb4_unicode_ci,
  `pollVotes` longtext COLLATE utf8mb4_unicode_ci,
  `messageIds` longtext COLLATE utf8mb4_unicode_ci,
  `anony` bit(1) DEFAULT NULL,
  `closed` bit(1) DEFAULT NULL,
  `archived` bit(1) DEFAULT NULL,
  `pollType` int(8) DEFAULT NULL,
  `people` longtext COLLATE utf8mb4_unicode_ci,
  `lang` int(8) NOT NULL DEFAULT '0',
  `maxVotes` int(32) DEFAULT NULL,
  `percentageBar` varchar(45) COLLATE utf8mb4_unicode_ci DEFAULT 'none',
  `sorted` bit(1) DEFAULT b'0',
  `appendable` bit(1) DEFAULT b'0',
  PRIMARY KEY (`chatid`,`pollid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;