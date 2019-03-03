# Documentation

We tried to combine incredible things - tracking users in different places, analyzing data using algorithms from marketing and creating tips on how best to engage the user. We could go further if there was a category with Cortex :) What we love about hackathons is that we can experiment!

## Summary

### Category:

Best use of xConnect and/or Universal Tracker

### Module Purpose: 

The purpose of this module is to divide visitors into different segments using Universal tracking and xConnect to identify the most interested and most uninterested in customers. We will try to increase response rates and convert more prospects into happy customers  and prevent the clients churn  through personalized mailing and advert

### Algorithm

The algorithm of grouping users is based on the adapted RFM analysis borrowed from marketing. Segmentation occurs in 27 different groups.

![rfm](https://github.com/Sitecore-Hackathon/2019-NaN/blob/master/documentation/images/rfm.jpg)

Unlike the original RFM analysis we use Goal Value instead of the price. The most business valuable group has the maximum RFM coefficient, users who use the service a little or are going to leave – the minimum. In the future after segmentation, we can send to users personalized offers, display personalized advertising for greater involvement and sales.


Read more: https://en.wikipedia.org/wiki/RFM_(customer_value)

### How it works

Using the Universal Tracker, we collect the values of the event from various resources. The value is the ratio (number) of how important the action was made by the customer. All of this data is captured by the xConnect and then processed by the Processing Engine. During event processing, we segment customers, identify their "business value" and store it.

![process](https://github.com/Sitecore-Hackathon/2019-NaN/blob/master/documentation/images/process.jpg)

## Pre-requisites

Module has no dependencies on other specific modules. Since we implement our module in the category "Best use of xConnect and/or Universal Tracker" for correct work, you need the following instances:

- Sitecore Experience Platform 9.1 Initial Release
- xConnect
- Universal tracker

You will also need frameworks:
- .NET Core 2.1
- .NET Framework 4.7.1

## Installation & Configuration

Perform the following steps to install the module:

- Use the Sitecore Installation Wizard to install the packages
- Upload demo data using the console application

First of all, you need an installed instance of Sitecore 9.1 and Universal tracker. 


### Step 1:
Install the sitecore package [BusinessValueTracker.SitecorePackage.zip](https://github.com/Sitecore-Hackathon/2019-NaN/blob/master/sc.package/BusinessValueTracker.SitecorePackage.zip.zip)

### Step 2:
Unpack the archive [BusinessValueTracker.ConfigDeploy.zip](https://github.com/Sitecore-Hackathon/2019-NaN/blob/master/sc.package/BusinessValueTracker.ConfigDeploy.zip) to any folder.
Update "xconnect" variable in "xConnectConfigs.Deploy.ps1" script with correct path to your xConnect instance.
Run the script from Power Shell.

If the script fails, copy the:

'ContactModel, 1.0.json' file to the following xConnect folders:

- \App_Data\jobs\continuous\ProcessingEngine\App_Data\Models"
- \App_Data\Models"
- \App_Data\jobs\continuous\IndexWorker\App_data\Models"

Files:
- sc.Demo.GoalsProjectionModel.Models.xml
- sc.Demo.Processing.Engine.ML.Workers.xml
- sc.Demo.Processing.Services.MLNet.xml

to the next directory:

\App_Data\jobs\continuous\ProcessingEngine\App_Data\Config\Sitecore\Processing

### Step 3
You need to make a modification to the file sc.XConnect.Client.xml which is located in the following directory:
```xml
{sc91.xconnect.sc}\App_Data\jobs\continuous\ProcessingEngine\App_Data\Config\Sitecore\XConnect\
```
where {sc91.xconnect.sc} - your xConnect directory

Insert the next code
```xml
<CustomModel>
	<TypeName>Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Xdb.XdbContactModel, Hackathon.Boilerplate.Foundation.BusinessValueTracker</TypeName>
	<PropertyName>Model</PropertyName>
</CustomModel>
```
in the section:
Settings/Sitecore/XConnect.Client/Client.ConfigurationServices/Client.Configuration/Options/Models/

### Step 4: 
Unpack the archive [Demo.zip](https://github.com/Sitecore-Hackathon/2019-NaN/blob/sc.package/Demo.zip) to any folder. 
Run the "InitialImport.ps1" script from Power Shell to import the demo data if your Collection host of Universal Tracker has a name 'http://sitecore.tracking.collection.service/'. You can edit the Power Shell script by adding the "-h " parameter with your own url. The demo data contains user generated events for Universal Tracker service.

It takes quite a long time from an hour to two. You can interrupt the process after 15-20 minutes by simply closing the console. This should be enough data for the demo. In this case, fewer clients will be recorded.

## Usage

To see how the module works, you can use Power Shell scripts:

- To simulate the most important business customers use the script GenerateMostValuableUser.ps1 from the Demo.zip
- To simulate the least valuable business customers customers use the script GenerateLeastValuableUser.ps1 from the Demo.zip


Each contact will be defined in the following groups depending on its actions:

### Leaving
Value | Group | Recommendations
--- | --- | ---
111 | Lost | Most likely these clients have already left. It makes sense to send them an automatic chain of letters with an offer to return.
112 | Single | Most likely these clients have already left. It makes sense to send them an automatic chain of letters with an offer to return.
113 | Single | Most likely these clients have already left. It makes sense to send them an automatic chain of letters with an offer to return.
121 | Leaving | These customers have already done some important actions on your service. You can try to get them back.
122 | Leaving | These customers have already done some important actions on your service. You can try to get them back.
123 | Leaving | These customers have already done some important actions on your service. You can try to get them back.
131 | Leaving - Permanent | These customers you need to try to be sure to return. Offer them bonuses and loyalty programs.
132 | Leaving  - Permanent good | These customers you need to try to be sure to return. Offer them bonuses and loyalty programs.
133 | Leaving - Permanent VIP | These customers you need to try to be sure to return. Offer them bonuses and loyalty programs.


### Sleeping
Value | Group | Recommendations
--- | --- | ---
211 | Sleeping | These customers remember your service. Try to engage them with promotions.
212 | Sleeping | These customers remember your service. Try to engage them with promotions.
213 | Sleeping | These customers remember your service. Try to engage them with promotions.
221 | Sleeping - Rare with low value | Maybe they're former clients. Find out why they are going to leave or left. We send them newsletters with interesting promotions.
222 | Sleeping - Rare with middle value | Maybe they're former clients. Find out why they are going to leave or left. We send them newsletters with interesting promotions.
223 | Sleeping - Rare with high value | Maybe they're former clients. Find out why they are going to leave or left. We send them newsletters with interesting promotions.
231 | Sleeping - Permanent with low value | These customers remember your service. Try to engage them with promotions.
232 | Sleeping - Permanent with middle value | These customers remember your service. Try to engage them with promotions.
233 | Sleeping - Permanent with high value | These customers remember your service. Try to engage them with promotions.

### Regular
Value | Group | Recommendations
--- | --- | ---
311 | Novice - Low value | We send a chain of letters with a description of the benefits and answers to questions.
312 | Novice with middle value | We send a chain of letters with a description of the benefits and answers to questions.
313 | Novice with high value | We send a chain of letters with a description of the benefits and answers to questions. Add an interesting offer to keep the interest.
321 | Regular with low value | We should try to increase their involvement.  We send them mails with related products and services.
322 | Regular - Middle value | We should try to increase their involvement.  We send them mails with related products and services.
323 | Regular - High value | These are good clients. Do not bore them with mailing. Only send a normal mails.
323 | Very regular - Low value | You can try to engage customers even more. We send them links to related products and services.
331 | Regular - High value | These are good clients. Do not bore them with mailing. Only send a normal mails.
332 | Very regular - Middle value | These are the best customers. You can try to sell them a new product or service. Send them special offers
333 | VIP | These are the best customers. You can try to sell them a new product or service. Send them special offers



## Tools

### Console utilite to generate interactions.
Paramters:

-h, --host \<url> - url to UT collection service. Default value: http://sitecore.tracking.collection.service/
    
-i, --import \<path> - generate interactions from provided csv file. Default value: interactions.csv
    
-g - provide this paramter to generate interactions from parameters

-n, --number \<int> - count of interactions to generate. Default value: 10
    
-c, --customerid \<int> - customerid to generate interactions for. If not provided generate a random id.
    
-s, --startdate \<date> - start date of time range. Used with --max and --min paramters. Default value: 12/1/2017
    
-e, --enddate \<date> - end date of time randge. Used with --max and --min paramters. Default value: 12/9/2018
    
--max - generate most valuable user with 10(default) interactions per month in time range. Parameters -n override count of interaction per month. -c can be provided

--min - generate least valuable user with 1 interaction during all time range. 

Examples:

`dotnet Hackathon.Boilerplate.Project.ConsoleGenerator.dll -g --max -n 20 -h "https://mysc.tracking.collection.service` - generates most valuable user with 20 interactions each month.

`dotnet Hackathon.Boilerplate.Project.ConsoleGenerator.dll -g -n 10 -c 1234` - generates 10 interactions for user 1234

`dotnet Hackathon.Boilerplate.Project.ConsoleGenerator.dll --import interactions.csv` - generates interactions from csv file "interactions.csv"
    


## Video


https://youtu.be/YjWoZx4GH_A
