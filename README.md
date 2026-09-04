1. Executive Summary & Project Mission
2. RACEDAY is a specialized, web-based athletic event management platform engineered explicitly for the South African road running, walking, and cycling communities. The platform serves as a dual-sided ecosystem: bridging the operational needs of event organizers with a seamless, data-driven experience for athletes. Organizers are empowered with comprehensive tools to deploy events, manage underlying athletic categories, and oversee athlete rosters. Simultaneously, participants can intuitively discover local fixtures, register for events, archive their historical race performance, and leverage real-time route dynamics alongside live localized weather data to safely prepare for their upcoming race day.


3
3.1 Functional Core Capabilities and User Roles
🏢 For Organizers
• The system must permanently capture and store each organizer’s unique identifier, legal first name, and surname.
•	Event Lifecycle Management: Tools to conceptualize, schedule, and edit multi-race sporting events.
•	Roster & Field Management: Live tracking of athlete fields, categories, and event participation limits.
👥 For Participants
• create own profile and then login using password, username and email
•	Discovery Engine: A localized search and filter interface to browse upcoming South African races by city, sport type, and date.
•	Athlete Portfolio: A personalized dashboard tracking historical finishes, personal bests, and active event statuses.
•	Race-Day Readiness Toolkit: Integration of live interactive route maps coupled with real-time weather tracking to optimize race preparation


4. Business rules:
 An organiser can create and manage multiple events.
• Each organiser's name and surname must be stored in the database.
• An organiser can create multiple race events.
• participant must register their profile using password,username,email before being allowed to enter event. 
• A participant may enter only one race at a time, as races within an event may take place simultaneously.
• Each participant's name, surname, age, and location must be stored in the database.
• Each event can contain multiple races. • Each event must include a title, description, and city must be stored in the database.
• Each Event must belong to one category, such as cycling, walking and running.
• Each event takes place in a specific city

ERD
<img width="940" height="952" alt="image" src="https://github.com/user-attachments/assets/af562667-efee-44d9-bc6a-66867a11488e" />


RESTFUL API
The restful api will enable organiser to create profile by registering themselves to the system
Once login the organiser can create events, delete events, update events and view events
He can also view participant’s information, events they have entered, delete and update participants. The workflow can be created to send email to notify participant of changes 
Participants must be able to Register own accounts then login
A RESTful API specification table
1 Organiser endpoint
HTTP methods	Route	description	Role required	Request body	response
GET	api/event/{eventI}	View event	organiser	{ title, description, date, location}	200 (OK), 404 (No Found)
POST	api/event	Create event,	Organiser	{ title, description, date, location}	200 (Created), 400 Bad Request
PUT	api/event/{eventId}	Update event 	Organiser	{ eventId}	201 Ok; 400 not found
DELETE	api/event/{eventId}	Delete event 	Organiser		204 (No Content) ,404 (No Found)
					
GET	api/categories/{categoryId}	View Category	Organiser	{categoryId, categoryname}	200 (Ok),400 (not found)
POST	/api/categories	Create Category	Organiser	{categoryname}	200 (Created)  400 Bad Request
Delete	/api/categories/{categoryId}	Delete Category	Organiser		204 (No Content) ,404 (No Found)
					
					
					
GET	/api/profiles/{participantId}	Capture participant results	Participant	{participantId, FirstName, Surname}	200 (Ok), 204 (No Found)
GET	/api/profiles/{participantId}	View profile	Participant	{participantId}	200 (Ok), 204 No Found
POST	/api/auth/users	Create profile account	Participant	{ username, password, email}	201 Created (Success)
400 (Bad Request (invalidate credentials)
POST	/api/auth/tokens	login	participant	{ username, password,}	200 OK (returns token), 401 Unauthorized
POST	api/event/{eventId}	View Events	Participant	{ title, description, date, location}	200(OK),404 (Not Found)
GET	/api/participants/{participantId}/registrations	View own entries	Participant	{ entriesId }	200 OK, 404 (Not Found), 204 (No Content)
GET	/api/participants/{participantIId}/results	Track own results	Participant	{participantId}	200 (Ok), 404 (Not Found), 204 (No Content)


- SQL Database Script

tables
<img width="940" height="829" alt="image" src="https://github.com/user-attachments/assets/5237b7ba-22d9-46c7-afed-c3eb2526cf3b" />


Step 2: Define keys and constraints
List of Entities and their attributes
1.	AuthUsers ( UserID PRIMARY KEY,
Username VARCHAR(50) NOT NULL, 
PasswordHash VARCHAR (255) NOT NULL,
 Email VARCHAR (100) NOT NULL,
 CONSTRAINT PK_AuthUsers PRIMARY KEY (UserID) );

2.	Organiser
OrganiserID PRIMARY KEY,
FirstName VARCHAR (20) NOT NULL,
Surname VARCHAR (20) NOT NULL,
3.	Participant
participantID PRIMARY KEY,
FirstName VARCHAR (20) NOT NULL,
Surname VARCHAR 20) NOT NULL,
Age int NOT NULL,
Location VARCHAR (20) NOT NULL,
4.	Categories
CategoryID PRIMARY KEY,
CategoryName VARCHAR(50) NOT NULL,
5.	Events[NM1.1]

•	[Event]
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    [Description] VARCHAR(255) NOT NULL,
    Title VARCHAR(90) NOT NULL,
    [Location] VARCHAR(20) NULL,
    OrganiserID INT NOT NULL,
    CategoryID INT NOT NULL,
    CONSTRAINT FK_Event_Organiser FOREIGN KEY (OrganiserID) REFERENCES Organiser(OrganiserID,
    CONSTRAINT FK_Event_Category FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);

6.	Races 
    RaceID INT IDENTITY(1,1) PRIMARY KEY,
    RaceName VARCHAR(50) NOT NULL,
    StartTime DATETIME NOT NULL,
    EventID INT NOT NULL,

7.	RaceEntries 
    EntryID INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantID INT NOT NULL,
    RaceID INT NOT NULL,
    EventID INT NOT NULL, 
    EntryDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_RaceEntries_Participant FOREIGN KEY (ParticipantID)      REFERENCES Participant(ParticipantID),
    CONSTRAINT FK_RaceEntries_Race FOREIGN KEY (RaceID) REFERENCES Races(RaceID),
    CONSTRAINT FK_RaceEntries_Event FOREIGN KEY (EventID) REFERENCES [Event](EventID) ON DELETE CASCADE,
    
    -- Enforces: "A participant may enter only one race per event at a time "
    CONSTRAINT UQ_Participant_Single_Race_Per_Event UNIQUE (ParticipantID, EventID)

Step 3 : Seed Data 

INSERT sample records
<img width="940" height="576" alt="image" src="https://github.com/user-attachments/assets/e331d33b-4dfd-4195-b7bb-062c6bf39933" />
<img width="940" height="247" alt="image" src="https://github.com/user-attachments/assets/47a42752-7109-4bbe-a0bd-8e44d74d967e" />




step 4: Testing the database and output: records
<img width="940" height="717" alt="image" src="https://github.com/user-attachments/assets/e56f1a79-d6ae-408c-a887-81fad3d3a683" />
<img width="940" height="332" alt="image" src="https://github.com/user-attachments/assets/88af9a11-a3f6-4881-b969-bd769780b904" />


Cardinality & Relationships 
Users ➔ Paricipant: One-to-one (1-1 to 1). Users must create user account and this will be linked with participant 
Organiser ➔ Event: One-to-Many (1-1 to 1*).An organizer creates one or many events; an event is created by exactly one organizer.
Categories ➔ Event: One-to-Many (1-1 to 1*). A category belongs to/contains one or many events.
Event ➔ Races: One-to-Many (1-1 to 1*). An event hosts one or many races.
Participant ➔ RaceEntries: One-to-Many (1 to 1*). A participant registers for one or many race entries.
Races ➔ RaceEntries: One-to-Many (1-1 to 1*). A race enters into one or many race entries.
Event ➔ RaceEntries: One-to-Many (1-1 to 1*). An event contains one or many race entries.




Explain the /docs folder
RaceDay Repository
|
|-- README.md
|
|-- Projectfiles
| |-- RacedatPOEPartERD.pdf
| |-- POEPart1ERD.pdf
| |- RacedayDb.sql
|
`-- .github
 `-- workflows

## Database Setup
Explain how to open and run the SQL script in SSMS.

## CI/CD
The GitHub Actions workflow checks if all the files uploaded on the repository are available with their correct file name.

<img width="1629" height="85" alt="image" src="https://github.com/user-attachments/assets/639b864e-a913-4d4b-abdb-0622b37d1cf7" />

workflow action
<img width="1080" height="648" alt="image" src="https://github.com/user-attachments/assets/29558fb8-3ff3-42be-85c9-47bf39962ef5" />


## Video Demonstration
YouTube Link: [https://youtu.be/mkcNyMAOg5M](https://youtu.be/EV9iX0_JtLU)
 



