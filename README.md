1. Executive Summary & Project Mission
2. RACEDAY is a specialized, web-based athletic event management platform engineered explicitly for the South African road running, walking, and cycling communities. The platform serves as a dual-sided ecosystem: bridging the operational needs of event organizers with a seamless, data-driven experience for athletes. Organizers are empowered with comprehensive tools to deploy events, manage underlying athletic categories, and oversee athlete rosters. Simultaneously, participants can intuitively discover local fixtures, register for events, archive their historical race performance, and leverage real-time route dynamics alongside live localized weather data to safely prepare for their upcoming race day.


3
3.1 Functional Core Capabilities and User Roles
🏢 For Organizers
• The system must permanently capture and store each organizer’s unique identifier, legal first name, and surname.
•	Event Lifecycle Management: Tools to conceptualize, schedule, and edit multi-race sporting events.
•	Roster & Field Management: Live tracking of athlete fields, categories, and event participation limits.
👥 For Participants
•	Discovery Engine: A localized search and filter interface to browse upcoming South African races by city, sport type, and date.
•	Athlete Portfolio: A personalized dashboard tracking historical finishes, personal bests, and active event statuses.
•	Race-Day Readiness Toolkit: Integration of live interactive route maps coupled with real-time weather tracking to optimize race preparation



ER

4. Business rules:
 An organiser can create and manage multiple events.
• Each organiser's name and surname must be stored in the database.
• An organiser can create multiple race events.
• A participant may enter only one race at a time, as races within an event may take place simultaneously.
• Each participant's name, surname, age, and location must be stored in the database.
• Each event can contain multiple races. • Each event must include a title, description, and city must be stored in the database.
• Each Event must belong to one category, such as cycling, walking and running.
• Each event takes place in a specific city
<img width="940" height="716" alt="image" src="https://github.com/user-attachments/assets/b9526368-1b29-473d-a6ec-8a39a47f8b48" />

Step 2: Define keys and constraints
List of Entities and their attributes
1.	Organiser
  OrganiserID PRIMARY KEY,
  FirstName varchar(20) NOT NULL,
  Surname varchar(20) NOT NULL,
2.	Participant
  participantID PRIMARY KEY,
  FirstName varchar(20) NOT NULL,
  Surname varchar(20) NOT NULL,
  Age int NOT NULL,
  Location varchar(20) NOT NULL,
3.	Categories
  CategoryID PRIMARY KEY,
  CategoryName VARCHAR(50) NOT NULL,
4.	Events
  	[Event]
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    [Description] VARCHAR(255) NOT NULL,
    Title VARCHAR(90) NOT NULL,
    [Location] VARCHAR(20) NULL,
    OrganiserID INT NOT NULL,
    CategoryID INT NOT NULL,
    CONSTRAINT FK_Event_Organiser FOREIGN KEY (OrganiserID) REFERENCES Organiser(OrganiserID,
    CONSTRAINT FK_Event_Category FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);

5.	Races 
    RaceID INT IDENTITY(1,1) PRIMARY KEY,
    RaceName VARCHAR(50) NOT NULL,
    StartTime DATETIME NOT NULL,
    EventID INT NOT NULL,

6.	RaceEntries 
    EntryID INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantID INT NOT NULL,
    RaceID INT NOT NULL,
    EventID INT NOT NULL, 
    EntryDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_RaceEntries_Participant FOREIGN KEY (ParticipantID)      REFERENCES Participant(ParticipantID),
    CONSTRAINT FK_RaceEntries_Race FOREIGN KEY (RaceID) REFERENCES Races(RaceID),


Step 3 : Seed Data 

INSERT sample records
<img width="940" height="279" alt="image" src="https://github.com/user-attachments/assets/e15a4b84-66a9-4b4a-8439-adafe6fecdf2" />
<img width="940" height="262" alt="image" src="https://github.com/user-attachments/assets/1e588423-4f02-49f8-a87b-259d9e0180ef" />



