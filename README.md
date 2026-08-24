1. Executive Summary & Project Mission
2. RACEDAY is a specialized, web-based athletic event management platform engineered explicitly for the South African road running, walking, and cycling communities. The platform serves as a dual-sided ecosystem: bridging the operational needs of event organizers with a seamless, data-driven experience for athletes. Organizers are empowered with comprehensive tools to deploy events, manage underlying athletic categories, and oversee athlete rosters. Simultaneously, participants can intuitively discover local fixtures, register for events, archive their historical race performance, and leverage real-time route dynamics alongside live localized weather data to safely prepare for their upcoming race day.

3. Business rules:
 An organiser can create and manage multiple events.
• Each organiser's name and surname must be stored in the database.
• An organiser can create multiple race events.
• A participant may enter only one race at a time, as races within an event may take place simultaneously.
• Each participant's name, surname, age, and location must be stored in the database.
• Each event can contain multiple races. • Each event must include a title, description, and city must be stored in the database.
• Each Event must belong to one category, such as cycling, walking and running.
• Each event takes place in a specific city
   
6. 2. Functional Core Capabilities
🏢 For Organizers
•	Event Lifecycle Management: Tools to conceptualize, schedule, and edit multi-race sporting events.
•	Roster & Field Management: Live tracking of athlete fields, categories, and event participation limits.
👥 For Participants
•	Discovery Engine: A localized search and filter interface to browse upcoming South African races by city, sport type, and date.
•	Athlete Portfolio: A personalized dashboard tracking historical finishes, personal bests, and active event statuses.
•	Race-Day Readiness Toolkit: Integration of live interactive route maps coupled with real-time weather tracking to optimize race preparation
3. Formalized Business Rules & System Constraints
To guarantee structural database integrity and clean business logic, the system must strictly adhere to the following domain rules, organized by operational module:
👤 3.1 Organiser Rules
•	3.1.1 Profile Retention: The system must permanently capture and store each organizer’s unique identifier, legal first name, and surname.
•	3.1.2 Operational Capacity: A single organizer is permitted to create, host, and manage an unrestricted number of distinct multi-race events.
📅 3.2 Event & Category Rules
•	3.2.1 Mandatory Profile Attributes: Every event profile compiled on the platform must explicitly store a title, a comprehensive description, and the designated host city.
•	3.2.2 Categorization Constraint: Every event must map to exactly one primary sporting classification archetype (e.g., Running, Walking, or Cycling).
•	3.2.3 Structural Composition: A single event acts as a parent container that can house multiple distinct sub-races (e.g., a single marathon event containing a 42km, 21km, and 5km fun run).
•	3.2.4 Localization: Every event is tied to a specific geographical city within South Africa to anchor weather and route discovery.
🏃 3.3 Participant & Registration Rules
•	3.3.1 Profile Retention: The system must record and maintain each participant’s first name, surname, age, and geographical home location.
•	3.3.2 Concurrency Protection (The Single-Entry Rule): Because multiple sub-races within a single parent event frequently occur simultaneously, a participant is strictly prohibited from entering more than one race per event concurrent window.
•	3.3.3 Historical Flexibility: A participant is free to register for an unlimited number of races over time across the system, provided those races belong to entirely separate events.

