CREATE DATABASE  IF NOT EXISTS `groupagreebot_beta` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci */;
USE `groupagreebot_beta`;
-- MySQL dump 10.13  Distrib 8.0.12, for Win64 (x86_64)
--
-- Host: localhost    Database: groupagreebot_beta
-- ------------------------------------------------------
-- Server version	8.0.12

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
 SET NAMES utf8 ;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `instances`
--

DROP TABLE IF EXISTS `instances`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `instances` (
  `chat_id` int(64) NOT NULL,
  `key` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `owner` mediumtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `uses_24` int(64) DEFAULT NULL,
  `uses_month` int(64) DEFAULT NULL,
  `creation_date` datetime DEFAULT NULL,
  `offset` int(64) NOT NULL DEFAULT '0',
  `last_30_updates` json DEFAULT NULL,
  `retry_at` int(64) DEFAULT NULL,
  PRIMARY KEY (`chat_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pointer`
--

DROP TABLE IF EXISTS `pointer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `polls`
--

DROP TABLE IF EXISTS `polls`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `polls` (
  `chatid` int(32) NOT NULL,
  `pollid` int(32) NOT NULL,
  `pollText` mediumtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `pollDescription` mediumtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `pollVotes` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `messageIds` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `anony` bit(1) DEFAULT NULL,
  `closed` bit(1) DEFAULT NULL,
  `archived` bit(1) DEFAULT NULL,
  `pollType` int(8) DEFAULT NULL,
  `people` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `lang` int(8) NOT NULL DEFAULT '0',
  `maxVotes` int(32) DEFAULT NULL,
  `percentageBar` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT 'none',
  `sorted` bit(1) DEFAULT b'0',
  `appendable` bit(1) DEFAULT b'0',
  PRIMARY KEY (`chatid`,`pollid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2019-05-05 16:11:38
