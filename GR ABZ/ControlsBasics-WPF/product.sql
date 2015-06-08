-- phpMyAdmin SQL Dump
-- version 4.2.11
-- http://www.phpmyadmin.net
--
-- Host: 127.0.0.1
-- Generation Time: Apr 19, 2015 at 06:54 PM
-- Server version: 5.6.21
-- PHP Version: 5.6.3

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `grabs`
--

-- --------------------------------------------------------

--
-- Table structure for table `product`
--

CREATE TABLE IF NOT EXISTS `product` (
  `Name` varchar(50) NOT NULL,
  `Price` int(11) NOT NULL,
  `Cat` varchar(50) NOT NULL,
  `Ava` varchar(2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `product`
--

INSERT INTO `product` (`Name`, `Price`, `Cat`, `Ava`) VALUES
('Coffee', 50, 'Beverage', 'Y'),
('Tea', 48, 'Beverage', 'Y'),
('Bourn Vita', 100, 'Beverage', 'Y'),
('Complan', 65, 'Beverage', 'Y'),
('Horlicks', 55, 'Beverage', 'Y'),
('Pedia Sure', 90, 'Beverage', 'Y'),
('Boost', 85, 'Beverage', 'Y'),
('Red Bull', 80, 'Beverage', 'Y'),
('Mountain Dew', 40, 'Beverage', 'Y'),
('Pepsi', 45, 'Beverage', 'Y'),
('Loreal', 350, 'Cosmetics', 'Y'),
('Park Avenue Soap', 45, 'Cosmetics', 'Y'),
('Ponds', 99, 'Cosmetics', 'Y'),
('Nivea Deo', 85, 'Cosmetics', 'Y'),
('Vaseline', 85, 'Cosmetics', 'Y'),
('Dove Soap', 35, 'Cosmetics', 'Y'),
('Cinthol Soap', 30, 'Cosmetics', 'Y'),
('Hamam', 35, 'Cosmetics', 'Y'),
('Lux', 45, 'Cosmetics', 'Y'),
('Axe ', 85, 'Cosmetics', 'Y'),
('Moong Dhal', 45, 'Misc', 'Y'),
('Orid Dhal', 65, 'Misc', 'Y'),
('Toor Dhal', 55, 'Misc', 'Y'),
('Ashirvad Atta', 70, 'Misc', 'Y'),
('Fortune Oil', 90, 'Misc', 'Y'),
('Chana Masala', 55, 'Misc', 'Y'),
('Pepsodent', 40, 'Misc', 'Y'),
('Colgate', 35, 'Misc', 'Y'),
('Kelloggs', 120, 'Misc', 'Y'),
('Pasta', 45, 'Misc', 'Y'),
('Dark Fantasy', 50, 'Snacks', 'Y'),
('Lays', 20, 'Snacks', 'Y'),
('Ferrero Rocher', 750, 'Snacks', 'Y'),
('Cake Mix', 150, 'Snacks', 'Y'),
('Noodles', 45, 'Snacks', 'Y'),
('Kurkure', 20, 'Snacks', 'Y'),
('Cheetos', 30, 'Snacks', 'Y'),
('Nutella', 320, 'Snacks', 'Y'),
('Choco Pie', 120, 'Snacks', 'Y'),
('Pringles', 70, 'Snacks', 'Y'),
('Carrots', 65, 'Vegetables', 'Y'),
('Cabbage', 40, 'Vegetables', 'Y'),
('Onion', 55, 'Vegetables', 'Y'),
('Beans', 35, 'Vegetables', 'Y'),
('Potato', 50, 'Vegetables', 'Y'),
('Chilli', 20, 'Vegetables', 'Y'),
('Beetroot', 52, 'Vegetables', 'Y'),
('Broccoli', 75, 'Vegetables', 'Y'),
('Spinach', 35, 'Vegetables', 'Y'),
('Capsicum', 35, 'Vegetables', 'Y'),
('Apple', 45, 'Fruits', 'Y'),
('Mangoes', 75, 'Fruits', 'Y'),
('Orange', 50, 'Fruits', 'Y'),
('Banana', 40, 'Fruits', 'Y'),
('Pineapple', 60, 'Fruits', 'Y'),
('Watermelon', 55, 'Fruits', 'Y'),
('Grapes', 35, 'Fruits', 'Y'),
('Jack Fruit', 65, 'Fruits', 'Y'),
('Blueberry', 75, 'Fruits', 'Y'),
('Kiwi', 45, 'Fruits', 'Y');

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
