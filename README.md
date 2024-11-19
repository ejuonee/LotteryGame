LotteryGame
===========

LotteryGame is a console-based lottery application that allows players to purchase tickets and participate in a lottery draw. The application includes features like player management, ticket processing, prize calculation, and a user interface for interacting with the lottery.

Table of Contents
-----------------

-   [Getting Started](#getting-started)
-   [Project Structure](#project-structure)
-   [Configuration](#configuration)
-   [Running the Application](#running-the-application)
-   [Running the Tests](#running-the-tests)

Getting Started
---------------

To get started with the LotteryGame application, follow these steps:

1.  **Clone the repository**:

    Copy

    `git clone https://github.com/ejuonee/LotteryGame.git`

2.  **Restore the NuGet packages**:
    -   Open the `BedeLottery.sln` solution file in your preferred IDE.
    -   Restore the NuGet packages by building the solution.

Project Structure
-----------------

The BedeLottery solution is organized into the following projects:

1.  **BedeLottery.Logic**:
    -   This project contains the core logic and functionality of the lottery application, including services for player management, ticket processing, prize calculation, and user interface.
    -   The project is divided into several directories, such as `Configuration`, `Interfaces`, `Models`, `Services`, and `Validation`, which group related classes and interfaces.
2.  **BedeLottery.UI**:
    -   This project represents the user interface of the application, which is implemented as a console-based application.
    -   It includes the `LotteryGame` class, which coordinates the different services to run the lottery, and the `ConsoleUserInterface` class, which handles user input and output.
    -   The `appsettings.json` file in this project contains the configuration settings for the lottery.
3.  **BedeLottery.UnitTests**:
    -   This project contains the unit tests for the various components of the BedeLottery application.
    -   The tests are organized by the services they target, such as `PlayerServiceTests`, `PrizeServiceTests`, and `TicketServiceTests`.
    -   The `ConsoleUserInterfaceTests` class tests the behavior of the console-based user interface.

Configuration
-------------

The BedeLottery application uses the `appsettings.json` file located in the `BedeLottery.UI` project to store the configuration settings. You can modify the following settings:

-   `MinPlayers`: The minimum number of players allowed in the lottery.
-   `MaxPlayers`: The maximum number of players allowed in the lottery.
-   `InitialBalance`: The initial balance each player starts with.
-   `TicketPrice`: The price of a single lottery ticket.
-   `MaxTicketsPerPlayer`: The maximum number of tickets a player can purchase.

Running the Application
-----------------------

To run the BedeLottery application, follow these steps:

1.  Open the `BedeLottery.sln` solution file in your preferred IDE.
2.  Locate the `Program.cs` file in the `BedeLottery.UI` project.
3.  Run the application to start the lottery game.

The application will guide you through the lottery process, allowing you to purchase tickets, view the results, and decide whether to play again.

Running the Tests
-----------------

To run the unit tests for the BedeLottery application, follow these steps:

1.  Open the `BedeLottery.sln` solution file in your preferred IDE.
2.  In the test explorer, you will find the various test classes, such as `PlayerServiceTests`, `PrizeServiceTests`, `TicketServiceTests`, and `ConsoleUserInterfaceTests`.
3.  Run the desired tests or the entire test suite.