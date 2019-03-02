# Documentation

## Summary

### Category:

Best use of xConnect and/or Universal Tracker

### Module Purpose: 

The purpose of this module is to divide visitors into different segments using Universal tracking and xConnect to identify the most interested and most uninterested in customers. We will try to increase response rates and convert more prospects into happy customers  and prevent the clients churn  through personalized mailing and advert

### Algorithm

The algorithm of grouping users is based on the adapted RFM analysis borrowed from marketing. Segmentation occurs in 27 different groups.

![rfm](https://github.com/Sitecore-Hackathon/2019-NaN/blob/master/documentation/images/rfm.jpg)

Unlike the original RFM analysis we will use Goal Value instead of the price. The most business valuable group has the maximum RFM coefficient, users who use the service a little or are going to leave – the minimum. In the future after segmentation, we can send to users personalized offers, display personalized advertising for greater involvement and sales.

Read more: https://en.wikipedia.org/wiki/RFM_(customer_value)

### How it works

Using the Universal Tracker, we collect the values of the event from various resources. The value is the ratio (number) of how important the action was made by the customer. All of this data is captured by the xConnect and then processed by the Processing Engine. During event processing, we segment customers, identify their "business value" and store it.

![process](https://github.com/Sitecore-Hackathon/2019-NaN/blob/master/documentation/images/process.jpg)

## Pre-requisites

Module has no dependencies on other specific modules. Since we implement our module in the category "Best use of xConnect and/or Universal Tracker" for correct work, you will need the following instances:

- xConnect
- Universal tracker

## Installation

Perform the following steps to install the module:

- Use the Sitecore Installation Wizard to install the package
- Upload demo data using the console application

## Configuration


## Usage


## Video

