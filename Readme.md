# SFS Demo Exercise

```A QUICK NOTE: I would advise you view this markdown with a local reader such as VS Code's markdown preview. This will allow you to see images and follow the links directly to relevant files from the Repo's root.```

This repository represents a well tested coding exercise sample based off the [exercise instructions linked here](.\supporting-files\Instructions.pdf). [There is a video session that can be found here (YouTube)](https://youtu.be/7p1_qcDhf2o) which does a walkthrough of the project among other topics outlined in the exercise. First 10 minutes are focused on the project walkthrough, second 10 minutes are focused on the exercise questions (sorry it was a bit longer than I hoped!)

At a high level it represents a project with 2 separate REST services. With one service relying on the other for data. The scenario is financially based and the first sevice can be thought of as the `ReportService` (For retieving credit reports) and the other as the `DebtService` (for calculating types of debts based off the credit reports).

## Basic Structure

There are 4 projects. 2 are the services, and the other 2 are tests for their respective service with the following project names:

* ReportServce - Service 1
* ReportService.Tests
* DebtService - Service 2
* DebtService.Tests

### Services

Each service follows a pretty similar pattern. Rather than going fully minimal API and banging the project out quickly - I structured each one as though it had an intended purpose beyond just the exercise. They're fairly basic with a few simple dependency injected components that host only http endpoints to avoid certificate fun.

#### Service Logical Layout

Data logic is separated into its own folder with respective folders for data layer concerns such as entities, services, etc.

Business logic is only split off in the `DebtService`. This is due to the calculations it performs vs the logic in the `ReportService` around record retrieval which is all data access based.

The Controllers represent all of the REST endpoints and each rely on Data and Business components to accomplish their actions.

Each project has variables in the `appsettings.json` that drive the 

### Tests

Tests were done in Xunit with Moq, they're fairly simple with a global using for xUnit at the root of each, a single reference to the parent project which it is testing, and moq. Any other libraries are domain specific.

## Setup

### Prerequisites
This project is best run using Visual Studio 2022. It will also require a version of the .Net 6 (Core) SDK. You could also use commands for `dotnet run` and `dotnet test` for the respective individual projects within the `FinancialRestServices` folder if desired (though not explained further). For triggering endpoints I recommend Postman (see the section `Triggering the Endpoints` below)

### How to Debug Both Services
Within the `FinancialRestServices` folder is the solution file for the project. Open the solution in Visual Studio. Once within Visual Studio, right click the root solution node in the solution explorer and select `Properties` from the dropdown. You should be able to select multiple start up projects as seen in the following image
![Visual Studio Soulution Startup Settings](.\supporting-files\vs-startup-projects.png)

Doing this should change your `Run` selection to `<Multiple Startup Projects>` You can verify, or change it accordingly by the drop down next to the Start button
![Visual Studio Run Target](.\supporting-files\vs-run-target-dropdown.png)

You can tell the two services will be running as it should open 2 new browser windows with fake paths. The one with the path of `svc1` represents Service #1 (ReportService) and the window with `svc2` represents Service #2 (DebtService)

## Testing the Endpoints

Typical .Net 6 projects allow you to bundle Swagger into each service. I opted not to do that in this project in an effort to keep it more clean and focused...I mildly regret that after the fact. Instead I opted to include a ["Postman"](https://www.postman.com/product/what-is-postman/) collection and environment that can be found in the `supporting-files` folder: 

* [SFS Demo Collection](.\supporting-files\SFS-Demo.postman_collection.json)
* [SFS Demo Environment](.\supporting-files\SFS-Demo-Env.postman_environment.json)

[Instructions on how to import them can be found here](https://learning.postman.com/docs/getting-started/importing-and-exporting-data/)

Once you do, you should see and `SFS Demo` collection and an `SFS Demo Env` you can utilize. The collection relies on the environment so be sure to select it when running requests within it.
![Properly configured Postman](.\supporting-files\postman-setup.png)


Start your services up, and then send some requests their way! If you have 404 issues, be sure to check the environments tab and ensure the service base urls match those of the running services.

## Using Your Own Data

Within the [exercise instructions](.\supporting-files\Instructions.pdf) was an initial test data json file used to build/test the application. I Embedded this file into the project. The file is located in the `ReportService - Service 1` project under the `Data` folder, inside `SeedData` as `creditData.json` and it's read in as JSON on project start based off the variables in `ReportServce - Service 1`'s `appsettings.json` file [located here](.\FinancialRestServices\ReportService\appsettings.json).

* This describes `SeedDataPath` the file name to look for (path realtive to where the application is running) 
* As well as `SeedDataRoot` which describes the property in the JSON object to use as the root for its datasource.

NOTE: Currently the seed data file is confiugured via Visual Studio to be of type "Content" with the action of copy to output directory "Always". This means if you modify the file name outside of visual studio you must update the `.csproj` file to match the new name of the file it should copy.

To use your own data, make sure it's in a JSON array format in accordance with the data structure outlined in the instructions and either replace the seed data in the file with your own, or if you have a file you don't wish to change you may be able to swap it in and change the settings to easily flip between the two.

## Final Thoughts

I took the opportunity with this coding exercise to pick up some new libraries I had never used before in a project. As a result it took me a bit longer to complete than I had hoped, but I really enjoyed both of the new libraries I utilized:

* JsonFlatFileDataStore
* Flurl

### JsonFlatFileDataStore

This was a really cool library for doing these types of quick demos. I was able to easily wire it up to the sample file without any modifications or scripts which was very cool. I also enjoyed the fact that it could be typed and combined with linq to satisfy the basic data access logic that this exercise required.

### Flurl

This was a really fun one. I had always hated the wrapping required when mocking the `HttpClient` in .Net Core in unit tests. I wasn't eager to do it again for this exercise. I'd used RestSharp in the past, but wanted something different. What I found _was_ quite different. Flurl is a neat library that allows for fluent request building via extension methods.

On the surface that sounds even worse for unit testing - but the `Flurl.Http` package also includes a whole suite for testing that's a little unconventional to standard mocking at first, but super easy to use. The fact that it also comes with some basic factories and extensive documentation outlining different dependency injecting scenarios and their benefits and drawbacks made me feel confident I could learn the best practices of it in no time. It was a refreshing way to unit test :)

