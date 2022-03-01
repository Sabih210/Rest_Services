-- phpMyAdmin SQL Dump
-- version 4.9.2
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Mar 01, 2022 at 11:15 AM
-- Server version: 10.4.10-MariaDB
-- PHP Version: 7.3.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `doc_store_management`
--

-- --------------------------------------------------------

--
-- Table structure for table `dsm_document`
--

CREATE TABLE `dsm_document` (
  `documentid` int(10) NOT NULL,
  `folderid` int(10) DEFAULT NULL,
  `document_name` varchar(50) DEFAULT NULL,
  `created_on` date DEFAULT NULL,
  `created_by` varchar(50) DEFAULT NULL,
  `updated_on` date DEFAULT NULL,
  `updated_by` varchar(50) DEFAULT NULL,
  `is_deleted` int(1) NOT NULL,
  `DOCUMENT_FILE_NAME` varchar(200) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `dsm_document`
--

INSERT INTO `dsm_document` (`documentid`, `folderid`, `document_name`, `created_on`, `created_by`, `updated_on`, `updated_by`, `is_deleted`, `DOCUMENT_FILE_NAME`) VALUES
(1, 1, 'Doc one11', '2022-02-27', 'scripts', '2022-02-27', 'API', 1, NULL),
(2, 1, 'Doc two', '2022-02-27', 'scripts', '2022-02-27', 'scripts', 0, NULL),
(3, 1, 'Doc one11', '2022-02-27', 'API', '2022-02-27', 'API', 0, NULL),
(4, 1, 'sabih pic', '2022-03-01', 'API', '2022-03-01', 'API', 0, 'Mine-Grey.jpg'),
(5, 1, 'sabih pic', '2022-03-01', 'API', '2022-03-01', 'API', 0, 'Mine-Grey.jpg');

-- --------------------------------------------------------

--
-- Table structure for table `dsm_folder`
--

CREATE TABLE `dsm_folder` (
  `folderid` int(10) NOT NULL,
  `folder_name` varchar(50) DEFAULT NULL,
  `created_on` date DEFAULT NULL,
  `created_by` varchar(50) DEFAULT NULL,
  `updated_on` date DEFAULT NULL,
  `updated_by` varchar(50) DEFAULT NULL,
  `is_deleted` int(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `dsm_folder`
--

INSERT INTO `dsm_folder` (`folderid`, `folder_name`, `created_on`, `created_by`, `updated_on`, `updated_by`, `is_deleted`) VALUES
(1, 'Customer Feedback', '2022-02-27', 'script', '2022-02-27', 'script', 0),
(2, 'TEST', '2022-02-27', 'Scripts', '2022-02-27', 'API', 1),
(3, 'TEST3', '2022-02-27', 'Scripts', '2022-02-27', 'api', 0),
(4, 'Test4', '2022-02-27', 'API', '2022-02-27', 'API', 0),
(5, 'Test5', '2022-02-27', 'API', '2022-02-27', 'API', 0),
(6, 'Test677', '2022-02-27', 'API', '2022-02-28', 'API', 0);

-- --------------------------------------------------------

--
-- Table structure for table `dsm_topic`
--

CREATE TABLE `dsm_topic` (
  `topicid` int(10) NOT NULL,
  `folderid` int(10) DEFAULT NULL,
  `documentid` int(10) DEFAULT NULL,
  `topic` varchar(50) DEFAULT NULL,
  `created_on` date DEFAULT NULL,
  `created_by` varchar(50) DEFAULT NULL,
  `updated_on` date DEFAULT NULL,
  `updated_by` varchar(50) DEFAULT NULL,
  `is_deleted` int(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `dsm_topic`
--

INSERT INTO `dsm_topic` (`topicid`, `folderid`, `documentid`, `topic`, `created_on`, `created_by`, `updated_on`, `updated_by`, `is_deleted`) VALUES
(1, 1, 1, 'SpekiLove!', '2022-02-27', 'Scripts', '2022-02-27', 'API', 0),
(2, 1, 2, 'SpekiLove!', '2022-02-27', 'Scripts', '2022-02-27', 'Scripts', 0),
(3, 1, 3, 'SpekiLove!', '2022-02-27', 'API', '2022-02-27', 'API', 0),
(4, 1, 4, 'sabih pic', '2022-03-01', 'API', '2022-03-01', 'API', 0),
(5, 1, 5, 'sabih pic', '2022-03-01', 'API', '2022-03-01', 'API', 0);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `dsm_document`
--
ALTER TABLE `dsm_document`
  ADD PRIMARY KEY (`documentid`),
  ADD KEY `fk_folderid` (`folderid`);

--
-- Indexes for table `dsm_folder`
--
ALTER TABLE `dsm_folder`
  ADD PRIMARY KEY (`folderid`);

--
-- Indexes for table `dsm_topic`
--
ALTER TABLE `dsm_topic`
  ADD PRIMARY KEY (`topicid`),
  ADD KEY `fk_folderid_1` (`folderid`),
  ADD KEY `fk_document_1` (`documentid`);

--
-- Constraints for dumped tables
--

--
-- Constraints for table `dsm_document`
--
ALTER TABLE `dsm_document`
  ADD CONSTRAINT `fk_folderid` FOREIGN KEY (`folderid`) REFERENCES `dsm_folder` (`folderid`);

--
-- Constraints for table `dsm_topic`
--
ALTER TABLE `dsm_topic`
  ADD CONSTRAINT `fk_document_1` FOREIGN KEY (`documentid`) REFERENCES `dsm_document` (`documentid`),
  ADD CONSTRAINT `fk_folderid_1` FOREIGN KEY (`folderid`) REFERENCES `dsm_folder` (`folderid`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
